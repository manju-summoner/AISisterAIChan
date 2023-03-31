#r "Rosalind.dll"
#load "SaveData.csx"
#load "ChatGPT.csx"
#load "CollisionParts.csx"
#load "GhostMenu.csx"
using Shiorose;
using Shiorose.Resource;
using Shiorose.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Shiorose.Resource.ShioriEvent;

partial class AISisterAIChanGhost : Ghost
{
    readonly static Dictionary<string, int[]> faceset = new Dictionary<string, int[]>()
    {
        ["普通"] = new int[] { 0, 1, 2, 3, 100, 101, 200, 201, 202, 300, 400, 500, 501, 700, 800, 801, 802 },
        ["恥ずかしい"] = new int[] { 10, 11, 12, 13, 210, 310, 311, 410, 710, 711, },
        ["驚き"] = new int[] { 20, 21, 22, 23, 24, 25, 26, 420, 421, 422, 423, 424, 425 },
        ["悲しい"] = new int[] { 30, 31, 32, 33, 130, 131, 230, 730, 830, 831, },
        ["呆れる"]  = new int[] { 40, 41, 42, 43, 80, 81, 82, 83, 84, 85, 140, 203, 204, 205, 280, 281, 282, 380, 503, 540, 541, 542, 543, 580, 581, 582, 701, 702, 705, 740, 741, 742, 743, 744, 803, 840, 880 },
        ["笑顔"] = new int[] { 50, 51, 150, 151, },
        ["照れる"] = new int[]{ 750, 751, },
        ["目を閉じる"] = new int[] { 60, 61, 160, 161, 360, 361, 502, 503, 760, 860, 861 },
        ["怒り"] = new int[] { 70, 71, 72, 73, 170, 171, 270, 272, 273, 274, 370, 570, 770, 870 },
        ["エッチなのは駄目"] = new int[] { 90, 91, 190, 290, 291, 292, 390, 391, 392 },//39.png
        ["恍惚"] = new int[] { 110, 862, 863 },
        ["青ざめる"] = new int[] { 426 },
    };
    Random random = new Random();
    ChatGPTTalk chatGPTTalk = null;
    string messageLog = "";
    double faceRate = 0;
    bool isNademachi = false;
    public AISisterAIChanGhost()
    {
        // 更新URL
        Homeurl = "https://manjubox.net/Install/ai_sister_ai_chan/";

        // 必ず読み込んでください
        _saveData = SaveDataManager.Load<SaveData>();

        SettingRandomTalk();

        Resource.SakuraPortalButtonCaption = () => "AI妹アイちゃん";
        SakuraPortalSites.Add(new Site("配布ページ", "https://manjubox.net/ai_sister_ai_chan/"));
        SakuraPortalSites.Add(new Site("ソースコード", "https://github.com/manju-summoner/AISisterAIChan"));

        Resource.SakuraRecommendButtonCaption = () => "宣伝！";
        SakuraRecommendSites.Add(new Site("ゆっくりMovieMaker4", "https://manjubox.net/ymm4/"));
        SakuraRecommendSites.Add(new Site("饅頭遣い", "https://twitter.com/manju_summoner"));
    }
    private void SettingRandomTalk()
    {
        RandomTalks.Add(RandomTalk.CreateWithAutoWait(() =>
        {
            BeginTalk("兄：なにか話して");
            return "";
        }));
    }
    public override string OnMouseClick(IDictionary<int, string> reference, string mouseX, string mouseY, string charId, string partsName, string buttonName, DeviceType deviceType)
    {
        var parts = CollisionParts.GetCollisionPartsName(partsName);
        if (parts != null && buttonName == "2")
            BeginTalk($"兄：（アイの{parts}をつまむ）");

        return base.OnMouseClick(reference, mouseX, mouseY, charId, partsName, buttonName, deviceType);
    }

    public override string OnMouseDoubleClick(IDictionary<int, string> reference, string mouseX, string mouseY, string charId, string partsName, string buttonName, DeviceType deviceType)
    {
        var parts = CollisionParts.GetCollisionPartsName(partsName);
        if (parts != null)
        {
            BeginTalk($"兄：（アイの{parts}をつつく）");
            return "";
        }
        else
        {
            return OpenMenu();
        }
    }

    protected override string OnMouseStroke(string partsName, DeviceType deviceType)
    {
        var parts = CollisionParts.GetCollisionPartsName(partsName);
        if (parts != null)
            BeginTalk($"兄：（アイの{parts}を撫でる）");

        return base.OnMouseStroke(partsName, deviceType);
    }
    public override string OnMouseWheel(IDictionary<int, string> reference, string mouseX, string mouseY, string wheelRotation, string charId, string partsName, Shiorose.Resource.ShioriEvent.DeviceType deviceType)
    {
        if (wheelRotation.StartsWith("-"))
        {
            if (partsName == CollisionParts.Shoulder)
                BeginTalk("兄：（アイを抱き寄せる）");
            else if (partsName == CollisionParts.TwinTail)
                BeginTalk("兄：（アイのツインテールを弄ぶ）");
            else
            {
                var parts = CollisionParts.GetCollisionPartsName(partsName);
                if (parts != null)
                    BeginTalk($"兄：（アイの{parts}を引っ張る）");
            }
        }
        else
        {
            if (partsName == CollisionParts.TwinTail)
                BeginTalk("兄：（アイのツインテールをフワフワと持ち上げる）");
            else if (partsName == CollisionParts.Skirt)
                BeginTalk("兄：（アイのスカートをめくる）");
            else
            {
                var parts = CollisionParts.GetCollisionPartsName(partsName);
                if (parts != null)
                    BeginTalk($"兄：（アイの{parts}をワシャワシャする）");
            }
        }

        return base.OnMouseWheel(reference, mouseX, mouseY, wheelRotation, charId, partsName, deviceType);
    }

    public override string OnMouseMove(IDictionary<int, string> reference, string mouseX, string mouseY, string wheelRotation, string charId, string partsName, DeviceType deviceType)
    {
        if(!isNademachi && partsName == CollisionParts.Head)
        {
            //撫で待ち
            isNademachi = true;
            return "\\s[101]";
        }
        return base.OnMouseMove(reference, mouseX, mouseY, wheelRotation, charId, partsName, deviceType);
    }

    public override string OnMouseLeave(IDictionary<int, string> reference, string mouseX, string mouseY, string charId, string partsName, DeviceType deviceType)
    {
        isNademachi = false;
        return base.OnMouseLeave(reference, mouseX, mouseY, charId, partsName, deviceType);
    }

    /*
    //撫でが呼ばれなくなるので一旦コメントアウト
    public override string OnMouseHover(IDictionary<int, string> reference, string mouseX, string mouseY, string charId, string partsName, Shiorose.Resource.ShioriEvent.DeviceType deviceType)
    {
        var parts = CollisionParts.GetCollisionPartsName(partsName);
        if (parts != null)
            BeginTalk($"兄：（アイの{parts}に手を添える）");
        return base.OnMouseHover(reference, mouseX, mouseY, charId, partsName, deviceType);
    }
    */



    public override string OnCommunicate(IDictionary<int, string> reference, string senderName = "", string script = "", IEnumerable<string> extInfo = null)
    {
        var sender = senderName == "user" || senderName == null ? "兄" : senderName;
        BeginTalk(sender + "：" + script);
        return "";
    }

    void BeginTalk(string message)
    {
        if (chatGPTTalk != null)
            return;

        faceRate = random.NextDouble();
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
" + ((SaveData)SaveData).AiProfile.Select(x => x.Key + "：" + x.Value).DefaultIfEmpty(string.Empty).Aggregate((a, b) => a + "\r\n" + b) + @"

# 兄のプロフィール
性別：男
関係性：アイの兄
性格：妹に甘いお兄ちゃん。妹のことをとても大切にしている。
一人称：お兄ちゃん
アイの呼び方：アイ
" + ((SaveData)SaveData).UserProfile.Select(x => x.Key + "：" + x.Value).DefaultIfEmpty(string.Empty).Aggregate((a, b) => a + "\r\n" + b) + @"

# その他の情報
日時：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss dddd") + @"

# 出力フォーマット
アイのセリフ：{アイのセリフ}
アイの表情：「普通」「恥ずかしい」「驚き」「悲しい」「呆れる」「笑顔」「照れる」「目を閉じる」「怒り」「エッチなのは駄目」「恍惚」「青ざめる」
会話継続：「継続」「終了」
" + Enumerable.Range(0, ((SaveData)SaveData).ChoiceCount).Select(x => "兄のセリフ候補" + (x + 1) + "：{兄のセリフ}").DefaultIfEmpty(string.Empty).Aggregate((a, b) => a + "\r\n" + b) + @"


# 会話ルール
会話継続が「終了」の場合、兄のセリフ候補は出力しないでください。

# 会話履歴
" + messageLog;

        if (((SaveData)SaveData).IsDevMode)
        {
            if (!Directory.Exists(".\\log"))
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
        if (canTalk && chatGPTTalk != null)
        {
            var talk = chatGPTTalk;
            var log = messageLog;
            if (!talk.IsProcessing)
            {
                chatGPTTalk = null;
                messageLog = string.Empty;
            }

            return BuildTalk(talk.Response, !talk.IsProcessing, log);
        }
        return base.OnSecondChange(reference, uptime, isOffScreen, isOverlap, canTalk, leftSecond);
    }
    public override string OnMinuteChange(IDictionary<int, string> reference, string uptime, bool isOffScreen, bool isOverlap, bool canTalk, string leftSecond)
    {
        if(canTalk && ((SaveData)SaveData).IsRandomIdlingSurfaceEnabled)
            return "\\s["+faceset["普通"][(int)random.Next(0, faceset["普通"].Length-1)]+"]";
        else
            return base.OnMinuteChange(reference, uptime, isOffScreen, isOverlap, canTalk, leftSecond);
    }

    string BuildTalk(string response, bool createChoices, string log)
    {
        try
        {
            if (((SaveData)SaveData).IsDevMode)
            {
                if (!Directory.Exists(".\\log"))
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

            if (!createChoices)
                return talkBuilder.Append($"\\_q...").LineFeed().Build();

            if (createChoices && string.IsNullOrEmpty(aiResponse))
                 return new TalkBuilder()
                    .Marker().AppendChoice("ログを見る").LineFeed()
                    .Marker().AppendChoice("会話を終える").LineFeed()
                    .Build()
                    .ContinueWith(id =>
                    {
                        if (id == "ログを見る")
                            return new TalkBuilder()
                            .Append("\\_q").Append(EscapeLineBreak(log)).LineFeed()
                            .Append(EscapeLineBreak(response)).LineFeed()
                            .HalfLine()
                            .Marker().AppendChoice("戻る")
                            .Build()
                            .ContinueWith(x =>
                            {
                                if (x == "戻る")
                                    return BuildTalk(response, createChoices, log);
                                return "";
                            });
                        return "";
                    });

            DeferredEventTalkBuilder deferredEventTalkBuilder = null;
            if (onichanResponse.Length > 0)
            {
                foreach (var choice in onichanResponse.Take(3))
                {
                    if (deferredEventTalkBuilder == null)
                        deferredEventTalkBuilder = AppendWordWrapChoice(talkBuilder, choice);
                    else
                        deferredEventTalkBuilder = AppendWordWrapChoice(deferredEventTalkBuilder, choice);
                }
                deferredEventTalkBuilder = deferredEventTalkBuilder.Marker().AppendChoice("自分で入力する").LineFeed().HalfLine();
            }

            if (deferredEventTalkBuilder == null)
                deferredEventTalkBuilder = talkBuilder.Marker().AppendChoice("ログを見る").LineFeed();
            else
                deferredEventTalkBuilder = deferredEventTalkBuilder.Marker().AppendChoice("ログを見る").LineFeed();

            return deferredEventTalkBuilder
                    .Marker().AppendChoice("会話を終える").LineFeed()
                    .Build()
                    .ContinueWith(id =>
                    {
                        if (onichanResponse.Contains(id))
                            BeginTalk($"{log}アイ：{aiResponse}\r\n兄：{id}");
                        if (id == "ログを見る")
                            return new TalkBuilder()
                            .Append("\\_q").Append(EscapeLineBreak(log)).LineFeed()
                            .Append(EscapeLineBreak(response)).LineFeed()
                            .HalfLine()
                            .Marker().AppendChoice("戻る")
                            .Build()
                            .ContinueWith(x =>
                            {
                                if (x == "戻る")
                                    return BuildTalk(response, createChoices, log);
                                return "";
                            });
                        if (id == "自分で入力する")
                            return new TalkBuilder().AppendUserInput().Build().ContinueWith(input =>
                            {
                                BeginTalk($"{log}アイ：{aiResponse}\r\n兄：{input}");
                                return "";
                            });
                        return "";
                    });
        }
        catch (Exception e)
        {
            return e.ToString();
        }
    }
    string EscapeLineBreak(string text)
    {
        return text.Replace("\r\n", "\\n").Replace("\n", "\\n").Replace("\r", "\\n");
    }
    string DeleteLineBreak(string text)
    {
        return text.Replace("\r\n", "").Replace("\n", "").Replace("\r", "");
    }
    string GetAIResponse(string response)
    {
        var lines = response.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
        var aiResponse = lines.FirstOrDefault(x => x.StartsWith("アイのセリフ：") || x.StartsWith("アイ："));
        if (aiResponse == null)
            return "";
        return aiResponse.Replace("アイのセリフ：", "").Replace("アイ：", "").Trim(' ', '「', '」');
    }
    string[] GetOnichanRenponse(string response)
    {
        var lines = response.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
        var onichanResponse = lines.Where(x => x.StartsWith("兄のセリフ候補") || x.StartsWith("兄：")).ToArray();
        if (onichanResponse == null)
            return new string[] { };
        return onichanResponse.Select(x => x.Replace("兄のセリフ候補1：", "").Replace("兄のセリフ候補2：", "").Replace("兄のセリフ候補3：", "").Replace("兄：", "").Trim(' ', '「', '」')).ToArray();
    }
    int GetSurfaceId(string response)
    {
        var lines = response.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
        var face = lines.FirstOrDefault(x => x.StartsWith("アイの表情："));
        if (face is null)
            return 0;

        foreach(var set in faceset)
        {
            if (face.Contains(set.Key))
                return set.Value[(int)Math.Min(set.Value.Length * faceRate, set.Value.Length - 1)];
        }

        return 0;
    }
    DeferredEventTalkBuilder AppendWordWrapChoice(TalkBuilder builder, string text)
    {
        builder = builder.Marker();
        DeferredEventTalkBuilder deferredEventTalkBuilder = null;
        foreach (var choice in WordWrap(text))
        {
            if (deferredEventTalkBuilder == null)
                deferredEventTalkBuilder = builder.AppendChoice(choice, text).LineFeed();
            else
                deferredEventTalkBuilder = deferredEventTalkBuilder.AppendChoice(choice, text).LineFeed();
        }
        return deferredEventTalkBuilder;
    }
    DeferredEventTalkBuilder AppendWordWrapChoice(DeferredEventTalkBuilder builder, string text)
    {
        builder = builder.Marker();
        foreach (var choice in WordWrap(text))
            builder = builder.AppendChoice(choice, text).LineFeed();
        return builder;
    }
    IEnumerable<string> WordWrap(string text)
    {
        var width = 24;
        for (int i = 0; i < text.Length; i += width)
        {
            if (i + width < text.Length)
                yield return text.Substring(i, width);
            else
                yield return text.Substring(i);
        }
    }
}

return new AISisterAIChanGhost();