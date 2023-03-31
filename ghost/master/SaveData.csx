#r "Rosalind.dll"
using Shiorose;
using Shiorose.Resource;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[DataContract]
public class SaveData : BaseSaveData {
    /* BaseSaveDataで定義済み */
    // int TalkInterval => ランダムトークの間隔
    // string UserName => ユーザ名

    // 項目追加したい場合の定義はこんな感じ
    [DataMember]
    public string APIKey { get; set; }

    [DataMember]
    public int ChoiceCount { get; set; } = 2;

    [DataMember]
    public Dictionary<string,string> AiProfile{ get; set; } = new Dictionary<string, string>();

    [DataMember]
    public Dictionary<string,string> UserProfile{ get; set; } = new Dictionary<string, string>();

    [DataMember]
    public bool IsDevMode { get; set; } = false;

    [DataMember]
    public bool IsRandomIdlingSurfaceEnabled { get; set; } = true;

    /// <summary>
    /// デフォルト値はここで設定
    /// ただしsavedataのファイルがある際は初期化されないので、
    /// 後からメンバを増やした際は注意！！
    /// </summary>
    public SaveData()
    {
        UserName = "お兄ちゃん";
        TalkInterval = 300;
    }
}

// SaveFileの名前を変えたい場合
// SaveDataManager.SaveFileName = "save.json";