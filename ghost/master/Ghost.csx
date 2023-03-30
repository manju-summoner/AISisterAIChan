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
        BeginTalk(script);
        return "";
    }

    void BeginTalk(string message)
    {
        if(chatGPTTalk != null)
            return;
        
        messageLog = message + "\r\n";
        
        var prompt = "以下プロフィールの妹のセリフをシミュレートしてください。\r\n"
+"\r\n"
+"# 妹のプロフィール\r\n"
+"名前：アイ\r\n"
+"性別：女\r\n"
+"年齢：14\r\n"
+"関係性：ユーザーの妹\r\n"
+"性格：元気溌剌でクラスの人気者。兄に対しては心を許しているので、絶対に敬語を使わない。\r\n"
+"外見：ピンクの髪。ピンク色のリボンで髪を縛ってツインテールにしてる。全体的に華奢。\r\n"
+"服装：黒の長袖Tシャツにピンクのフリルミニスカート（2段）\r\n"
+"一人称：私\r\n"
+"兄の呼び方：おにいちゃん\r\n"
+"\r\n"
+"# 兄（ユーザー）のプロフィール\r\n"
+"性別：男\r\n"
+"性格：妹に甘いお兄ちゃん。妹のことをとても大切にしている。\r\n"
+"一人称：お兄ちゃん\r\n"
+"アイの呼び方：アイ\r\n"
+"\r\n"
+"# 出力フォーマット\r\n"
+"アイのセリフ：{アイのセリフ}\r\n"
+"アイの表情：「普通」「うーん」「えぇ…」「にっこり」「恥ずかしい」「照れ隠し」「悲しい」「驚き」「恍惚」「しらけ」「だるー」「まじで！？」「びっくり」「むっつり」「いやそれは」\r\n"
+"兄のセリフ候補1：{兄のセリフ}\r\n"
+"兄のセリフ候補2：{兄のセリフ}\r\n"
+"兄のセリフ候補3：{兄のセリフ}\r\n"
+"会話を打ち切ったほうが自然な場合はアイのセリフや兄の返答の選択肢を出力しないでください。\r\n"
+"\r\n"
+"# 会話ログ\r\n"
+messageLog;

        //System.IO.File.WriteAllText("D:\\prompt.txt", prompt);

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
        //System.IO.File.WriteAllText("D:\\response.txt", response);

        var aiResponse = GetAIResponse(response);
        var onichanResponse = GetOnichanRenponse(response);
        var surfaceId = GetSurfaceId(response);
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
                    deferredEventTalkBuilder = talkBuilder.Marker().AppendChoice(choice).LineFeed();
                else
                    deferredEventTalkBuilder = deferredEventTalkBuilder.Marker().AppendChoice(choice).LineFeed();
            }
        }
        if(deferredEventTalkBuilder == null)
            deferredEventTalkBuilder = talkBuilder.Marker().AppendChoice("回答を入力する").LineFeed().HalfLine();
        else
            deferredEventTalkBuilder = deferredEventTalkBuilder.Marker().AppendChoice("回答を入力する").LineFeed().HalfLine();

        deferredEventTalkBuilder = deferredEventTalkBuilder
            .Marker().AppendChoice("ログを見る").LineFeed()
            .Marker().AppendChoice("会話を打ち切る").LineFeed();

        return deferredEventTalkBuilder.Build().ContinueWith(id=>
        {
            if(onichanResponse.Contains(id))
                BeginTalk($"{log}アイ：{aiResponse}\r\n兄：{id}");
            if(id == "ログを見る")
                return new TalkBuilder().Append("\\_q").Append(EscapeLineBreak(log)).LineFeed().Append(EscapeLineBreak(response)).Build();
            if(id == "回答を入力する")
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
    string EscapeLineBreak(string text){
        return text.Replace("\r\n","\\n").Replace("\n","\\n").Replace("\r","\\n");
    }
    string DeleteLineBreak(string text){
        return text.Replace("\r\n","").Replace("\n","").Replace("\r","");
    }
    string GetAIResponse(string response){
        var lines = response.Split(new string[]{"\r\n", "\n", "\r"}, StringSplitOptions.None);
        var aiResponse = lines.FirstOrDefault(x=>x.StartsWith("アイのセリフ："));
        if(aiResponse == null)
            return "";
        return aiResponse.Replace("アイのセリフ：", "").Trim(' ','「','」');
    }
    string[] GetOnichanRenponse(string response){
        var lines = response.Split(new string[]{"\r\n", "\n", "\r"}, StringSplitOptions.None);
        var onichanResponse = lines.Where(x=>x.StartsWith("兄のセリフ候補")).ToArray();
        if(onichanResponse == null)
            return new string[]{};
        return onichanResponse.Select(x=>x.Replace("兄のセリフ候補1：", "").Replace("兄のセリフ候補2：", "").Replace("兄のセリフ候補3：", "").Trim(' ','「','」')).ToArray();
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
}

return new AISisterAIChanGhost();