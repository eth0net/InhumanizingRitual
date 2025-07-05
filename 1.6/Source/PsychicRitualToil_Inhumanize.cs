using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace InhumanizingRitual;

public class PsychicRitualToil_Inhumanize : PsychicRitualToil
{
    public PsychicRitualRoleDef invokerRole;

    public PsychicRitualRoleDef targetRole;

    public int comaDurationTicks;

    protected PsychicRitualToil_Inhumanize() { }

    public PsychicRitualToil_Inhumanize(PsychicRitualRoleDef invokerRole, PsychicRitualRoleDef targetRole)
    {
        this.invokerRole = invokerRole;
        this.targetRole = targetRole;
    }

    public override void Start(PsychicRitual psychicRitual, PsychicRitualGraph parent)
    {
        base.Start(psychicRitual, parent);
        var def = (PsychicRitualDef_Inhumanize)psychicRitual.def;
        var invoker = psychicRitual.assignments.FirstAssignedPawn(invokerRole);
        var target = psychicRitual.assignments.FirstAssignedPawn(targetRole);
        if (invoker == null || target == null)
        {
            return;
        }
        comaDurationTicks = Mathf.RoundToInt(def.comaDurationDaysFromQualityCurve.Evaluate(psychicRitual.PowerPercent) * 60000f);
        ApplyOutcome(psychicRitual, invoker, target);
    }

    private void ApplyOutcome(PsychicRitual psychicRitual, Pawn invoker, Pawn target)
    {
        target.health.AddHediff(HediffDefOf.Inhumanized);
        TaggedString text = "InhumanizeCompleteText".Translate(invoker.Named("INVOKER"), psychicRitual.def.Named("RITUAL"), target.Named("TARGET"));
        if (comaDurationTicks == 0)
        {
            target.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.HumanityBreak, "InhumanizeMentalBreak".Translate(target.Named("TARGET")));
            text += "\n\n" + "InhumanizeMentalBreak".Translate(target.Named("TARGET"));
        }
        else
        {
            var hediff = HediffMaker.MakeHediff(HediffDefOf.InhumanizeComa, target);
            hediff.TryGetComp<Verse.HediffComp_Disappears>()?.SetDuration(comaDurationTicks);
            target.health.AddHediff(hediff);
            text += "\n\n" + "InhumanizeTargetComa".Translate(target.Named("TARGET"), comaDurationTicks.ToStringTicksToDays()).CapitalizeFirst();
        }
        Find.LetterStack.ReceiveLetter("PsychicRitualCompleteLabel".Translate(psychicRitual.def.label), text, LetterDefOf.NeutralEvent, new LookTargets(invoker, target));
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Defs.Look(ref invokerRole, "invokerRole");
        Scribe_Defs.Look(ref targetRole, "targetRole");
        Scribe_Values.Look(ref comaDurationTicks, "comaDurationTicks");
    }
}
