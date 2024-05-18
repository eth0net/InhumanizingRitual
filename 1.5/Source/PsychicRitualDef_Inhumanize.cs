using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace InhumanizingRitual;

public class PsychicRitualDef_Inhumanize : PsychicRitualDef_InvocationCircle
{
    public SimpleCurve comaDurationDaysFromQualityCurve;

    public override List<PsychicRitualToil> CreateToils(PsychicRitual psychicRitual, PsychicRitualGraph parent)
    {
        var list = base.CreateToils(psychicRitual, parent);
        list.Add(new PsychicRitualToil_Inhumanize(InvokerRole, TargetRole));
        list.Add(new PsychicRitualToil_TargetCleanup(InvokerRole, TargetRole));
        return list;
    }

    public override TaggedString OutcomeDescription(FloatRange qualityRange, string qualityNumber, PsychicRitualRoleAssignments assignments)
    {
        return outcomeDescription.Formatted(Mathf.FloorToInt(comaDurationDaysFromQualityCurve.Evaluate(qualityRange.min) * 60000f).ToStringTicksToDays());
    }
}
