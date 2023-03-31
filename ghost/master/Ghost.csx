#r "Rosalind.dll"
#load "SaveData.csx"
#load "ChatGPT.csx"
using Shiorose;
using Shiorose.Resource;
using Shiorose.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

partial class AISisterAIChanGhost : Ghost
{
    ChatGPTTalk chatGPTTalk = null;
    string messageLog = "";
    public AISisterAIChanGhost()
    {
        // 更新URL
        Homeurl = "";

        // 必ず読み込んでください
        _saveData = SaveDataManager.Load<SaveData>();

        SettingRandomTalk();

        Resource.ReadmeButtonCaption = () => Util.RandomChoice("読んでね", "README");
        Resource.VanishButtonCaption = () => Util.RandomChoice("アンインストール", "アンインストール");
    }
    private void SettingRandomTalk()
    {
        RandomTalks.Add(RandomTalk.CreateWithAutoWait(()=> {
            BeginTalk("兄：なにか話して");
            return "";
        }));
    }

    public override string OnCommunicate(IDictionary<int, string> reference, string senderName = "", string script = "", IEnumerable<string> extInfo = null)
    {
        var sender = senderName == "user" || senderName == null ? "兄" : senderName;
        BeginTalk(sender + "：" + script);
        return "";
    }

    void BeginTalk(string message)
    {
        if(chatGPTTalk != null)
            return;
        
        messageLog = message + "\r\n";
        
        var prompt = @"以下のプロフィールと会話履歴を元に、アイのセリフのシミュレート結果を1つ出力してください。

# アイのプロフィール
名前：アイ
性別：女
年齢：14
関係性：妹
性格：元気溌剌でクラスの人気者。兄に対しては心を許しているので、絶対に敬語を使わない。
外見：ピンクの髪。ピンク色のリボンで髪を縛ってツインテールにしてる。全体的に華奢。
服装：黒の長袖Tシャツにピンクのフリルミニスカート（2段）
一人称：私
兄の呼び方：おにいちゃん
"+((SaveData)SaveData).AiProfile.Select(x=>x.Key+"："+x.Value).DefaultIfEmpty(string.Empty).Aggregate((a,b)=>a+"\r\n"+b)+@"

# 兄のプロフィール
性別：男
関係性：アイの兄
性格：妹に甘いお兄ちゃん。妹のことをとても大切にしている。
一人称：お兄ちゃん
アイの呼び方：アイ
"+((SaveData)SaveData).UserProfile.Select(x=>x.Key+"："+x.Value).DefaultIfEmpty(string.Empty).Aggregate((a,b)=>a+"\r\n"+b)+@"

# その他の情報
日時："+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss dddd")+@"

# 出力フォーマット
アイのセリフ：{アイのセリフ}
アイの表情：「普通」「うーん」「えぇ…」「にっこり」「恥ずかしい」「照れ隠し」「悲しい」「驚き」「恍惚」「しらけ」「だるー」「まじで！？」「びっくり」「むっつり」「いやそれは」
会話継続：「継続」「終了」
"+Enumerable.Range(0, ((SaveData)SaveData).ChoiceCount).Select(x=>"兄のセリフ候補"+(x+1)+"：{兄のセリフ}").DefaultIfEmpty(string.Empty).Aggregate((a,b)=>a+"\r\n"+b)+@"


# 会話ルール
会話継続が「終了」の場合、兄のセリフ候補は出力しないでください。

# 会話履歴
"+messageLog;

        if(((SaveData)SaveData).IsDevMode)
        {
            if(!Directory.Exists(".\\log"))
                Directory.CreateDirectory(".\\log");
            File.WriteAllText(".\\log\\prompt.txt", prompt);
        }

        var request = new ChatGPTRequest()
        {
            stream = true,
            model = "gpt-3.5-turbo",
            messages = new ChatGPTMessage[]
            {
                new ChatGPTMessage()
                {
                    role = "user",
                    content = prompt
                },
            }
        };
        chatGPTTalk = new ChatGPTTalk(((SaveData)SaveData).APIKey, request);
    }

    public override string OnSecondChange(IDictionary<int, string> reference, string uptime, bool isOffScreen, bool isOverlap, bool canTalk, string leftSecond)
    {
        if(canTalk && chatGPTTalk != null)
        {
            var talk = chatGPTTalk;
            var log = messageLog;
            if(!talk.IsProcessing)
            {
                chatGPTTalk = null;
                messageLog = string.Empty;
            }

            return BuildTalk(talk.Response, !talk.IsProcessing, log);
        }
        return base.OnSecondChange(reference, uptime, isOffScreen, isOverlap, canTalk, leftSecond);
    }

    string BuildTalk(string response, bool createChoices, string log){
        try{
        
        if(((SaveData)SaveData).IsDevMode)
        {
            if(!Directory.Exists(".\\log"))
                Directory.CreateDirectory(".\\log");
            File.WriteAllText(".\\log\\response.txt", response);
        }

        var aiResponse = GetAIResponse(response);
        var surfaceId = GetSurfaceId(response);
        var onichanResponse = GetOnichanRenponse(response);
        var talkBuilder = 
            new TalkBuilder()
            .Append($"\\_q\\s[{surfaceId}]")
            .Append(aiResponse)
            .LineFeed()
            .HalfLine();
        DeferredEventTalkBuilder deferredEventTalkBuilder = null;
        if(!createChoices)
            return talkBuilder.Append($"\\_q...").LineFeed().Build();
        if(createChoices && string.IsNullOrEmpty(aiResponse))
            return "";
        if(onichanResponse.Length > 0)
        {
            foreach(var choice in onichanResponse.Take(3))
            {
                if(deferredEventTalkBuilder == null)
                    deferredEventTalkBuilder = AppendWordWrapChoice(talkBuilder, choice);
                else
                    deferredEventTalkBuilder = AppendWordWrapChoice(deferredEventTalkBuilder, choice);
            }
            deferredEventTalkBuilder = deferredEventTalkBuilder.Marker().AppendChoice("自分で入力する").LineFeed().HalfLine();
        }

        if(deferredEventTalkBuilder == null)
            deferredEventTalkBuilder = talkBuilder.Marker().AppendChoice("ログを見る").LineFeed();
        else
            deferredEventTalkBuilder = deferredEventTalkBuilder.Marker().AppendChoice("ログを見る").LineFeed();
        
        return deferredEventTalkBuilder
                .Marker().AppendChoice("会話を終える").LineFeed()
                .Build()
                .ContinueWith(id=>
                {
                    if(onichanResponse.Contains(id))
                        BeginTalk($"{log}アイ：{aiResponse}\r\n兄：{id}");
                    if(id == "ログを見る")
                        return new TalkBuilder()
                        .Append("\\_q").Append(EscapeLineBreak(log)).LineFeed()
                        .Append(EscapeLineBreak(response)).LineFeed()
                        .HalfLine()
                        .Marker().AppendChoice("戻る")
                        .Build()
                        .ContinueWith(x=>
                        {
                            if(x=="戻る")
                                return BuildTalk(response, createChoices, log);
                            return "";
                        });
                    if(id == "自分で入力する")
                        return new TalkBuilder().AppendUserInput().Build().ContinueWith(input=>
                        {
                            BeginTalk($"{log}アイ：{aiResponse}\r\n兄：{input}");
                            return "";
                        });
                    return "";
                });
        }catch(Exception e)
        {
            return e.ToString();
        }
    }
    string EscapeLineBreak(string text)
    {
        return text.Replace("\r\n","\\n").Replace("\n","\\n").Replace("\r","\\n");
    }
    string DeleteLineBreak(string text)
    {
        return text.Replace("\r\n","").Replace("\n","").Replace("\r","");
    }
    string GetAIResponse(string response)
    {
        var lines = response.Split(new string[]{"\r\n", "\n", "\r"}, StringSplitOptions.None);
        var aiResponse = lines.FirstOrDefault(x=>x.StartsWith("アイのセリフ：") || x.StartsWith("アイ："));
        if(aiResponse == null)
            return "";
        return aiResponse.Replace("アイのセリフ：", "").Replace("アイ：", "").Trim(' ','「','」');
    }
    string[] GetOnichanRenponse(string response)
    {
        var lines = response.Split(new string[]{"\r\n", "\n", "\r"}, StringSplitOptions.None);
        var onichanResponse = lines.Where(x=>x.StartsWith("兄のセリフ候補") || x.StartsWith("兄：")).ToArray();
        if(onichanResponse == null)
            return new string[]{};
        return onichanResponse.Select(x=>x.Replace("兄のセリフ候補1：", "").Replace("兄のセリフ候補2：", "").Replace("兄のセリフ候補3：", "").Replace("兄：", "").Trim(' ','「','」')).ToArray();
    }
    int GetSurfaceId(string response)
    {
        var lines = response.Split(new string[]{"\r\n", "\n", "\r"}, StringSplitOptions.None);
        var face = lines.FirstOrDefault(x=>x.StartsWith("アイの表情："));
        if(face is null)
            return 0;

        //「普通」「うーん」「えぇ…」「にっこり」「恥ずかしい」「照れ隠し」「悲しい」「驚き」「恍惚」「しらけ」「だるー」「まじで！？」「びっくり」「むっつり」「いやそれは」
        if(face.Contains("普通"))
            return 0;
        if(face.Contains("うーん"))
            return 1;
        if(face.Contains("えぇ…"))
            return 2;
        if(face.Contains("にっこり"))
            return 3;
        if(face.Contains("恥ずかしい"))
            return 4;
        if(face.Contains("照れ隠し"))
            return 5;
        if(face.Contains("悲しい"))
            return 6;
        if(face.Contains("驚き"))
            return 7;
        if(face.Contains("恍惚"))
            return 8;
        if(face.Contains("しらけ"))
            return 9;
        if(face.Contains("だるー"))
            return 12;
        if(face.Contains("まじで！？"))
            return 13;
        if(face.Contains("びっくり"))
            return 14;
        if(face.Contains("むっつり"))
            return 15;
        if(face.Contains("いやそれは"))
            return 16;
        return 0;
    }
    DeferredEventTalkBuilder AppendWordWrapChoice(TalkBuilder builder, string text)
    {
        builder = builder.Marker();
        DeferredEventTalkBuilder deferredEventTalkBuilder = null;
        foreach(var choice in WordWrap(text))
        {
            if(deferredEventTalkBuilder == null)
                deferredEventTalkBuilder = builder.AppendChoice(choice, text).LineFeed();
            else
                deferredEventTalkBuilder = deferredEventTalkBuilder.AppendChoice(choice, text).LineFeed();
        }
        return deferredEventTalkBuilder;
    }
    DeferredEventTalkBuilder AppendWordWrapChoice(DeferredEventTalkBuilder builder, string text)
    {
        builder = builder.Marker();
        foreach(var choice in WordWrap(text))
            builder = builder.AppendChoice(choice, text).LineFeed();
        return builder;
    }
    IEnumerable<string> WordWrap(string text)
    {
        var width = 24;
        for(int i=0;i<text.Length;i+=width)
        {
            if(i+width < text.Length)
                yield return text.Substring(i, width);
            else
                yield return text.Substring(i);
        }
    }
}

return new AISisterAIChanGhost();