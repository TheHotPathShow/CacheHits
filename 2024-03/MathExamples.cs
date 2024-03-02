using Knackelibang;
using Unity.Entities;
using Unity.Mathematics;

public class MathExamples : SystemBase
{
    protected override void OnUpdate()
    {
        // values xyzw
        var value2D = new float2(1, 2);
        var value3DA = new float3(1, 2, 3);
        var value3DB = new float3(4, 5, 6);
        var value4D = new float4(1, 2, 3, 4);

        value2D = value3DA.xz;

        value4D.xyz = value3DA;
        value4D.xyw = value3DA;
        value4D.zyx = value3DA;

        var x = value4D[0];
        var y = value4D[1];
        var z = value4D[2];
        var w = value4D[3];

        var test = value2D.To3DXZ(15);

        // rads deg
        var rads = math.radians(90);
        var degs = math.degrees(math.PI);

        // safe
        var norm = math.normalizesafe(new float3(0));

        // directions
        var forward = math.forward();
        var right = math.right();
        var up = math.up();

        // Dir to rot
        var dir = new float3(3, 7, 9);
        var refUp = math.up();
        if (refUp.Equals(dir))
            refUp = math.forward();

        var rot = quaternion.LookRotation(dir, refUp);

        // dist sq
        var distSq1 = math.distancesq(value3DA, value3DB);
        var distSq2 = math.distancesq(value3DA, new float3(0, 6, 3));

        if (distSq1 < distSq2)
        {
            // Do stuff
        }

        // noise
        var randSeed = 12345;
        var threadIndex = 4;
        var entityIndex = 7;
        var time = 43.65f;
        var rand1 = noise.cellular(new float2(randSeed, threadIndex));
        var rand4 = noise.cellular2x2(new float2(randSeed, entityIndex));
        var rand2 = noise.cnoise(new float2(randSeed, time));
        var rand3 = noise.snoise(new float2(entityIndex, time));
    }
}

public static class MathExt
{
    public static float3 ToXZ3D(this float3 value, float y)
    {
        return new float3(value.x, y, value.y);
    }
}