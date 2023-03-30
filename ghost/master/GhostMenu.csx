#r "Rosalind.dll"
#load "SaveData.csx"
using Shiorose;
using Shiorose.Resource;
using Shiorose.Resource.ShioriEvent;
using Shiorose.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

partial class AISisterAIChanGhost : Ghost
{
    public override string OnMouseDoubleClick(IDictionary<int, string> reference, string mouseX, string mouseY, string charId, string partsName, string buttonName, DeviceType deviceType)
    {
        switch (partsName)
        {
            default:
                return OpenMenu();
        }
    }

    private string OpenMenu()
    {
        if(string.IsNullOrEmpty(((SaveData)SaveData).APIKey))
            return ChangeChatGPTAPITalk();

        const string RAND = "なにか話して";
        const string COMMUNICATE = "話しかける";
        const string CHANGEPROFILE = "プロフィールを変更する";
        const string SETTINGS = "設定を変えたい";
        const string CANCEL = "なんでもない";

        return new TalkBuilder().Append("どうしたの？").LineFeed()
                                .HalfLine()
                                .Marker().AppendChoice(RAND).LineFeed()
                                .Marker().AppendChoice(COMMUNICATE).LineFeed()
                                .HalfLine()
                                .Marker().AppendChoice(CHANGEPROFILE).LineFeed()
                                .Marker().AppendChoice(SETTINGS).LineFeed()
                                .HalfLine()
                                .Marker().AppendChoice(CANCEL)
                                .BuildWithAutoWait()
                                .ContinueWith((id) =>
                                {
                                    switch (id)
                                    {
                                        case RAND:
                                            return OnRandomTalk();
                                        case COMMUNICATE:
                                            return new TalkBuilder().Append("なになに？").AppendCommunicate().Build();
                                        case CHANGEPROFILE:
                                            return ChangeProfileTalk();
                                        case SETTINGS:
                                            return SettingsTalk();
                                        default:
                                            return new TalkBuilder().Append("そう…？").BuildWithAutoWait();
                                    }
                                });
    }

    private string SettingsTalk(){
        const string CHANGE_CHATGPT_API = "ChatGPTのAPIキーを変更する";
        const string CHANGE_RANDOMTALK_INTERVAL = "ランダムトークの頻度を変更する";
        const string CHANGE_CHOICE_COUNT = "選択肢の数を変更する";
        string CHANGE_DEVMODE = "開発者モードを変更する（現在："+(((SaveData)SaveData).IsDevMode ? "有効" : "無効")+"）";
        const string BAKC = "戻る";
        return new TalkBuilder()
        .Append("設定を変更するね。")
        .LineFeed()
        .HalfLine()
        .Marker().AppendChoice(CHANGE_CHATGPT_API).LineFeed()
        .Marker().AppendChoice(CHANGE_RANDOMTALK_INTERVAL).LineFeed()
        .Marker().AppendChoice(CHANGE_CHOICE_COUNT).LineFeed()
        .Marker().AppendChoice(CHANGE_DEVMODE).LineFeed()
        .HalfLine()
        .Marker().AppendChoice(BAKC)
        .BuildWithAutoWait()
        .ContinueWith(id=>
        {
            if (id == CHANGE_CHATGPT_API)
                return ChangeChatGPTAPITalk();
            else if (id == CHANGE_RANDOMTALK_INTERVAL)
                return ChangeRandomTalkIntervalTalk();
            else if (id == CHANGE_CHOICE_COUNT)
                return ChangeChoiceCountTalk();
            else if (id == CHANGE_DEVMODE)
            {
                ((SaveData)SaveData).IsDevMode = !((SaveData)SaveData).IsDevMode;
                return SettingsTalk();
            }
            else
                return OpenMenu();
        });
    }

    private string ChangeChatGPTAPITalk(){
        return new TalkBuilder().Append("ChatGPTのAPIキーを入力してね、おにいちゃん。")
                                .AppendPassInput(defValue:((SaveData)SaveData).APIKey)
                                .Build()
                                .ContinueWith(apiKey=>
                                {
                                    ((SaveData)SaveData).APIKey = apiKey;
                                    return new TalkBuilder().Append("設定が終わったよ、おにいちゃん。").BuildWithAutoWait();
                                });
    }

    private string ChangeRandomTalkIntervalTalk(){
        return new TalkBuilder().Append("ランダムトークの頻度を変更するよ。")
                                .LineFeed()
                                .HalfLine()
                                .Marker().AppendChoice("10秒").LineFeed()
                                .Marker().AppendChoice("30秒").LineFeed()
                                .Marker().AppendChoice("1分").LineFeed()
                                .Marker().AppendChoice("5分").LineFeed()
                                .Marker().AppendChoice("10分").LineFeed()
                                .Marker().AppendChoice("30分").LineFeed()
                                .Marker().AppendChoice("1時間").LineFeed()
                                .Marker().AppendChoice("なんでもない")
                                .BuildWithAutoWait()
                                .ContinueWith(id=>
                                {
                                    switch(id)
                                    {
                                        case "10秒":
                                            SaveData.TalkInterval = 10;
                                            return new TalkBuilder().Append("10秒に1回話すね。").BuildWithAutoWait();
                                        case "30秒":
                                            SaveData.TalkInterval = 30;
                                            return new TalkBuilder().Append("30秒に1回話すね。").BuildWithAutoWait();
                                        case "1分":
                                            SaveData.TalkInterval = 60;
                                            return new TalkBuilder().Append("1分に1回話すね。").BuildWithAutoWait();
                                        case "5分":
                                            SaveData.TalkInterval = 300;
                                            return new TalkBuilder().Append("5分に1回話すね。").BuildWithAutoWait();
                                        case "10分":
                                            SaveData.TalkInterval = 600;
                                            return new TalkBuilder().Append("10分に1回話すね。").BuildWithAutoWait();
                                        case "30分":
                                            SaveData.TalkInterval = 1800;
                                            return new TalkBuilder().Append("30分に1回話すね。").BuildWithAutoWait();
                                        case "1時間":
                                            SaveData.TalkInterval = 3600;
                                            return new TalkBuilder().Append("1時間に1回話すね。").BuildWithAutoWait();
                                        default:
                                            return new TalkBuilder().Append("また何か変えたくなったら呼んでね。").BuildWithAutoWait();
                                    }
                                });
    }
    private string ChangeChoiceCountTalk(){
        return new TalkBuilder()
            .Append("会話時の選択肢の数を変更するよ。").LineFeed()
            .Append("……選択肢って何？").LineFeed().HalfLine()
            .Marker().AppendChoice("0個").LineFeed()
            .Marker().AppendChoice("1個").LineFeed()
            .Marker().AppendChoice("2個").LineFeed()
            .Marker().AppendChoice("3個").LineFeed()
            .Marker().AppendChoice("変更しない").LineFeed()
            .BuildWithAutoWait()
            .ContinueWith(id=>
            {
                switch(id){
                    case "0個":
                        ((SaveData)SaveData).ChoiceCount = 0;
                        return new TalkBuilder().Append("選択肢を表示しないようにするよ。").BuildWithAutoWait();
                    case "1個":
                        ((SaveData)SaveData).ChoiceCount = 1;
                        return new TalkBuilder().Append("選択肢を1個表示するよ。").BuildWithAutoWait();
                    case "2個":
                        ((SaveData)SaveData).ChoiceCount = 2;
                        return new TalkBuilder().Append("選択肢を2個表示するよ。").BuildWithAutoWait();
                    case "3個":
                        ((SaveData)SaveData).ChoiceCount = 3;
                        return new TalkBuilder().Append("選択肢を3個表示するよ。").BuildWithAutoWait();
                    default:
                        return new TalkBuilder().Append("また何か変えたくなったら呼んでね。").BuildWithAutoWait();
                }
            });
    }
    private string ChangeProfileTalk()
    {
        return new TalkBuilder().Append("どっちのプロフィールを変更する？").LineFeed()
                                .HalfLine()
                                .Marker().AppendChoice("アイ").LineFeed()
                                .Marker().AppendChoice("おにいちゃん").LineFeed()
                                .HalfLine()
                                .Marker().AppendChoice("戻る")
                                .Build()
                                .ContinueWith(id=>
                                {
                                    switch(id)
                                    {
                                        case "アイ":
                                            return ChangeProfileDictionaryTalk(((SaveData)SaveData).AiProfile, "わたし");
                                        case "おにいちゃん":
                                            return ChangeProfileDictionaryTalk(((SaveData)SaveData).UserProfile, "おにいちゃん");
                                        default:
                                            return OpenMenu();
                                    }
                                });
    }
    private string ChangeProfileDictionaryTalk(Dictionary<string,string> profile, string targetName)
    {
        try{
        DeferredEventTalkBuilder deferredEventTalkBuilder = null;
        var builder = new TalkBuilder()
            .Append("\\_q")
            .Append(targetName + "のプロフィールを変更するよ。").LineFeed()
            .HalfLine();
        
        foreach(var pair in profile)
        {
            if(deferredEventTalkBuilder == null)
                deferredEventTalkBuilder = builder.Marker().AppendChoice(pair.Key+"："+TrimLength(pair.Value,10), pair.Key).Marker().AppendChoice("削除","削除"+pair.Key).LineFeed();
            else
                deferredEventTalkBuilder = deferredEventTalkBuilder.Marker().AppendChoice(pair.Key+"："+TrimLength(pair.Value,10), pair.Key).Marker().AppendChoice("削除","削除"+pair.Key).LineFeed();
        }
        if(deferredEventTalkBuilder == null)
            deferredEventTalkBuilder = builder.Marker().AppendChoice("項目を追加する").LineFeed();
        else
            deferredEventTalkBuilder = deferredEventTalkBuilder.Marker().AppendChoice("項目を追加する").LineFeed();

        return deferredEventTalkBuilder
            .HalfLine()
            .Marker().AppendChoice("戻る").LineFeed()
            .BuildWithAutoWait()
            .ContinueWith(id=>
            {
                if(id == "戻る")
                    return ChangeProfileTalk();
                else if(id == "項目を追加する")
                    return AddProfileTalk(profile, id);
                else if(id.StartsWith("削除"))
                {
                    profile.Remove(id.Substring(2));
                    return ChangeProfileDictionaryTalk(profile, targetName);
                }
                else
                    return ChangeProfileDetailTalk(profile, id);
            });
        }catch(Exception e)
        {
            return e.ToString();
        }
    }
    private string ChangeProfileDetailTalk(Dictionary<string,string> profile, string key)
    {
        return new TalkBuilder().Append(key + "の内容を変更するよ。").LineFeed()
                                .HalfLine()
                                .AppendUserInput(defValue: profile.ContainsKey(key) ? profile[key] : "")
                                .Marker().AppendChoice("戻る").LineFeed()
                                .BuildWithAutoWait()
                                .ContinueWith(id=>
                                {
                                    if(id != "戻る")
                                        profile[key] = id;
                                    return ChangeProfileDictionaryTalk(profile, profile == ((SaveData)SaveData).AiProfile ? "わたし" : "おにいちゃん");
                                });
    }
    private string AddProfileTalk(Dictionary<string,string> profile, string targetName)
    {
        return new TalkBuilder().Append("追加する項目の名前を入力してね。").LineFeed()
                                .HalfLine()
                                .AppendUserInput()
                                .Marker().AppendChoice("戻る").LineFeed()
                                .BuildWithAutoWait()
                                .ContinueWith(id=>
                                {
                                    if(id != "戻る")
                                        return ChangeProfileDetailTalk(profile, id);
                                    else
                                        return ChangeProfileDictionaryTalk(profile, profile == ((SaveData)SaveData).AiProfile ? "アイ" : "おにいちゃん");
                                });
    }
    private string TrimLength(string text, int maxLength){
        if(text.Length > maxLength)
            return text.Substring(0, maxLength) + "…";
        else
            return text;
    }
}
