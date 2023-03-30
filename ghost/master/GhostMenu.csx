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
        const string SETTINGS = "設定を変えたい";
        const string CANCEL = "なんでもない";

        return new TalkBuilder().Append("どうしたの？").LineFeed()
                                .HalfLine()
                                .Marker().AppendChoice(RAND).LineFeed()
                                .Marker().AppendChoice(COMMUNICATE).LineFeed()
                                .HalfLine()
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
        const string CANCEL = "なんでもない";
        return new TalkBuilder()
        .Append("設定を変更するね。")
        .LineFeed()
        .HalfLine()
        .Marker().AppendChoice(CHANGE_CHATGPT_API).LineFeed()
        .Marker().AppendChoice(CHANGE_RANDOMTALK_INTERVAL).LineFeed()
        .Marker().AppendChoice(CHANGE_CHOICE_COUNT).LineFeed()
        .HalfLine()
        .Marker().AppendChoice(CANCEL)
        .BuildWithAutoWait()
        .ContinueWith(id=>
        {
            switch(id)
            {
                case CHANGE_CHATGPT_API:
                    return ChangeChatGPTAPITalk();
                case CHANGE_RANDOMTALK_INTERVAL:
                    return ChangeRandomTalkIntervalTalk();
                case CHANGE_CHOICE_COUNT:
                    return ChangeChoiceCountTalk();
                default:
                    return new TalkBuilder().Append("また何か変えたくなったら呼んでね。").BuildWithAutoWait();
            }
        });
    }

    private string ChangeChatGPTAPITalk(){
        return new TalkBuilder().Append("ChatGPTのAPIキーを入力してね、おにいちゃん。")
                                .AppendPassInput(defValue:((SaveData)SaveData).APIKey)
                                .Build()
                                .ContinueWith(apiKey=>
                                {
                                    ((SaveData)SaveData).APIKey = apiKey;
                                    return new TalkBuilder().Append("設定が終わったよ、お兄ちゃん。").BuildWithAutoWait();
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
}
