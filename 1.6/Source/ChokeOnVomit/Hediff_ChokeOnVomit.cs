using RimWorld;
using Verse;

namespace ChokeOnVomit
{
    public class Hediff_ChokeOnVomit : HediffWithComps
    {
        private float intervalFactor;

        private const int SeverityChangeInterval = 1000;

        private const float TendSuccessChanceFactor = 2.5f;

        private const float TendSeverityReduction = 0.4f;

        public override void PostMake()
        {
            base.PostMake();
            intervalFactor = Rand.Range(1f, 2.5f);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref intervalFactor, "intervalFactor", 0f);
        }

        public override void TickInterval(int delta)
        {
            base.TickInterval(delta);
            if (!pawn.IsHashIntervalTick((int)(SeverityChangeInterval * intervalFactor), delta)) return;
            float change = Rand.Range(-0.3f, 0.5f);
            Severity += change;
            if (change >= 0) return;
            FilthMaker.TryMakeFilth(pawn.Position, pawn.Map, ThingDefOf.Filth_Vomit, pawn.LabelIndefinite());
            //SoundDefOf.Vomit.TrySpawnSustainer(new TargetInfo(pawn.Position, pawn.Map));
            //EffecterDefOf.Vomit.Spawn(pawn.Position, pawn.Map);
        }

        public override void Tended(float quality, float maxQuality, int batchPosition = 0)
        {
            base.Tended(quality, maxQuality, batchPosition);
            float num = TendSuccessChanceFactor * quality;
            if (Rand.Value < num)
            {
                if (batchPosition == 0 && pawn.Spawned)
                {
                    MoteMaker.ThrowText(pawn.DrawPos, pawn.Map, "TextMote_TreatSuccess".Translate(num.ToStringPercent()), 6.5f);
                }
                Severity -= TendSeverityReduction;
                FilthMaker.TryMakeFilth(pawn.Position, pawn.Map, ThingDefOf.Filth_Vomit, pawn.LabelIndefinite());
                //SoundDefOf.Vomit.TrySpawnSustainer(new TargetInfo(pawn.Position, pawn.Map));
                //EffecterDefOf.Vomit.Spawn(pawn.Position, pawn.Map);
            }
            else if (batchPosition == 0 && pawn.Spawned) MoteMaker.ThrowText(pawn.DrawPos, pawn.Map, "TextMote_TreatFailed".Translate(num.ToStringPercent()), 6.5f);
        }
    }
}
