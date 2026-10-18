namespace Lab1;

public static class EffectiveBranching
{
    public static double Compute(double n, int depth)
    {
        double target = n + 1;
        double lo = 0, hi = target;

        for (int i = 0; i < 200; i++)
        {
            double mid = (lo + hi) / 2;
            if (Sum(mid, depth) < target) lo = mid; else hi = mid;
        }

        return (lo + hi) / 2;
    }

    private static double Sum(double b, int depth)
    {
        double s = 1;
        for (int i = 1; i <= depth; i++) s = 1 + b * s;
        return s;
    }
}