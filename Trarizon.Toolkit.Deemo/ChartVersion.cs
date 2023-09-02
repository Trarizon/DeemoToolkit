namespace Trarizon.Toolkit.Deemo;
public enum ChartVersion
{
    /// <summary>
    /// The most standard and simplest format for DEEMO
    /// </summary>
    /// <remarks>
    /// speed, <br/>
    /// notes[ id, sounds?[ w?, d?, p, v ], pos?, size, _time? ], <br/>
    /// links[ note[ ref ] ]
    /// </remarks>
    Deemo = 0x0000,
    /// <summary>
    /// The complex format for DEEMO,
    /// wth of these weird unused properties
    /// </summary>
    /// <remarks>
    /// speed,  <br/>
    /// note[ id, type, sounds?[ w, d, p, v ], pos, size, _time, shift, time ],  <br/>
    /// links[ `ditto` ]
    /// </remarks>
    DeemoV2 = 0x0001,
    /// <summary>
    /// The most weird format for DEEMO,
    /// we have to parse it manually.<br/>
    /// Yes it makes Newtonsoft.Json failed.
    /// </summary>
    /// <remarks>
    /// notes[ _time, id, sounds[ d, p, v, w ], pos, size ],  <br/>
    /// links[ `ditto` ],  <br/>
    /// speed<br/>
    /// # property position changed. <br/>
    /// ! id:str->int
    /// </remarks>
    DeemoV3 = 0x0002,
    /// <summary>
    /// The format for DEEMO -Reborn-,
    /// similar to <see cref="DeemoV2"/>
    /// </summary>
    /// <remarks>
    /// speed, <br/>
    /// notes[ id, type, sound[w, d, p, v], pos, size, _time, shift, vibrate, eventID, time ], <br/>
    /// links[ `ditto` ]
    /// </remarks>
    Reborn = 0x1000,
    /// <summary>
    /// The first format for DEEMO II
    /// </summary>
    /// <remarks>
    /// speed, oriVMin, oriVMax, remapVMin, remapVMax,  <br/>
    /// notes[id, sounds, pos, size, _time, shift, speed, duration, vibrate, warningType, eventID, time],  <br/>
    /// links[ `ditto` ], <br/>
    /// lines[speed, startTime, endTime, warningType] <br/>
    /// ! sounds is explicit null <br/>
    /// ! formatted
    /// </remarks>
    DeemoII = 0x2000,
    /// <summary>
    /// The second format for DEEMO II, 
    /// they added flick(swipe) here, 
    /// and warningType has a new meaning which this library now using
    /// </summary>
    /// <remarks>
    /// speed, oriVMin, oriVMax, remapVMin, remapVMax,  <br/>
    /// notes[ id, sounds, pos, size, _time, shift, speed, duration, vibrate, swipe, warningType, eventID, time ], <br/>
    /// links[ `ditto` ], <br/>
    /// lines[ speed, startTime, endTime, warningType ]
    /// </remarks>
    DeemoIIV2 = 0x2001,
}

public static class ChartVersionExtensions
{
    public static GameVersion ToGameVersion(this ChartVersion chartVer)
        => (GameVersion)chartVer & (GameVersion)0xf;
}