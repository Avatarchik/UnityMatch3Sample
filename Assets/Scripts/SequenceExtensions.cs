using DG.Tweening;

public static class SequenceExtensions
{
    public static Sequence InsertAtBegin(this Sequence sequence, Tween tween)
    {
        return sequence.Insert(0f, tween);
    }
}
