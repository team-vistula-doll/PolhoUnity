using System.Collections;
using System.Collections.Generic;
using DanmakU;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

public class TestModifier : MonoBehaviour, IDanmakuModifier
{
    public float WaveLength;
    public float BaseSpeed;
    public float WaveSpeed;

    public JobHandle UpdateDannmaku(DanmakuPool pool, JobHandle dependency = default(JobHandle))
    {
        return new ApplyWave()
        {
            NewSpeed = BaseSpeed+Mathf.Sin(Time.time*1f/WaveLength)*WaveSpeed, Speeds = pool.Speeds
        }.Schedule(pool.ActiveCount, DanmakuPool.kBatchSize, dependency);
    }

    struct ApplyWave : IJobParallelFor
    {
        public float NewSpeed { get; set; }
        public NativeArray<float> Speeds;

        public void Execute(int index)
        {
            Speeds[index] = NewSpeed;
        }
    }
}

