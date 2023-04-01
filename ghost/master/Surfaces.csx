#load "SurfaceCategory.csx"
using System;
using System.Collections.Generic;

public class Surfaces
{
    static Random random = new Random();
    static Dictionary<string, Surfaces> SurfaceList = new Dictionary<string, Surfaces>()
    {
        [SurfaceCategory.Normal] = new Surfaces( 0, 1, 2, 3, 100, 101, 200, 201, 202, 300, 400, 500, 501, 700, 800, 801, 802),
        [SurfaceCategory.Embarrassed] = new Surfaces( 11, 12, 13, 14, 210, 310, 311, 410, 710, 711 ),
        [SurfaceCategory.Surprise] = new Surfaces( 20, 21, 22, 23, 24, 25, 26, 420, 421, 422, 423, 424, 425 ),
        [SurfaceCategory.Sad] = new Surfaces( 30, 31, 32, 33, 130, 131, 230, 730, 830, 831),
        [SurfaceCategory.Amazed] = new Surfaces( 40, 41, 42, 43, 80, 81, 82, 83, 84, 85, 140, 203, 204, 205, 280, 281, 282, 380, 503, 540, 541, 542, 543, 580, 581, 582, 701, 702, 705, 740, 741, 742, 743, 744, 803, 840, 880 ),
        [SurfaceCategory.Smile] = new Surfaces( 50, 51, 150, 151),
        [SurfaceCategory.Bashful] = new Surfaces( 750, 751),
        [SurfaceCategory.CloseEyes] = new Surfaces( 60, 61, 160, 161, 360, 361, 502, 503, 760, 860, 861 ),
        [SurfaceCategory.Anger] = new Surfaces( 70, 71, 72, 73, 170, 171, 270, 272, 273, 274, 370, 570, 770, 870 ),
        [SurfaceCategory.Sensitive] = new Surfaces( 90, 91, 190, 290, 291, 292, 390, 391, 392 ),//39.png
        [SurfaceCategory.Ecstatic] = new Surfaces( 110, 862, 863 ),
        [SurfaceCategory.Pale] = new Surfaces( 426 ),
    };

    public static Surfaces Of(string category) => SurfaceList[category];

    int[] surfaces;
    public Surfaces(params int[] surfaces)
    {
        this.surfaces = surfaces;
    }
    public int GetRaodomSurface()
    {
        return surfaces[random.Next(surfaces.Length)];
    }
    public int GetSurfaceFromRate(double rate)
    {
        var index = Math.Min((int)(surfaces.Length * rate), surfaces.Length - 1);
        return surfaces[index];
    }

}