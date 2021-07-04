public struct Phase
{
    // このフェイズの時間(秒)
    public int time { get; }

    // 一回で同時に出現する数
    public int popCount { get; }

    // 出現率(0~1)
    public float popProb { get; }
    
    // 出現の間隔(秒)
    public int popInterval { get; }
    
    // 土偶の出現確率(0~1)
    public float popDoguProb { get; }

    public Phase(int time, int popCount, float popProb, int popInterval, float popDoguProb)
    {
        this.time = time;
        this.popCount = popCount;
        this.popProb = popProb;
        this.popInterval = popInterval;
        this.popDoguProb = popDoguProb;
    }
}