﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using rjw;
namespace RJW_Genes
{
    public class IncidentWorker_SuccubusDreamVisit : IncidentWorker
    {
        //This incidint will only fire if there is a pawn asleep which while sexneed is lower than 0.25
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                return false;
            }
            Map map = (Map)parms.target;
            if (!map.mapTemperature.SeasonAcceptableFor(ThingDefOf.Human))
            {
                return false;
            }
            foreach (Pawn pawn in map.mapPawns.FreeColonistsAndPrisonersSpawned)
            {
                if (pawn.jobs.curDriver.asleep && xxx.need_some_sex(pawn) > 1f)
                {
                    return true;
                }
            }
            return false;
            
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            Pawn victim = ValidVictims(map).RandomElement();
            if (victim == null)
            {
                return false;
            }
            IntVec3 loc = victim.Position;
            Faction faction;
            if (!this.TryFindFormerFaction(out faction))
            {
                return false;
            }

            //Spawn succubus at pawn and initiate sex
            Pawn succubus = PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDef.Named("rjw_genes_succubus"), faction, PawnGenerationContext.NonPlayer, -1, 
                false, false, false, true, false, 1f, false, true, false, true, true, false, false, false, false, 0f, 0f, null, 1f, null, null, 
                null, null, null, null, null, null, null, null, null, null, false, false, false, false, null, null, null, null, null, 0f,
                DevelopmentalStage.Adult, null, null, null, false));
            succubus.SetFaction(null, null);
            GenSpawn.Spawn(succubus, loc, map, WipeMode.Vanish);
            
            //Broken for now
            //Sends letter
            //string value = succubus.DevelopmentalStage.Child() ? "FeralChild".Translate().ToString() : succubus.KindLabel;
            //TaggedString value2 = succubus.DevelopmentalStage.Child() ? "Child".Translate() : "Person".Translate();
            //TaggedString baseLetterLabel = this.def.letterLabel.Formatted(value).CapitalizeFirst();
            //TaggedString baseLetterText = this.def.letterText.Formatted(succubus.NameShortColored, value2, succubus.Named("PAWN")).AdjustedFor(succubus, "PAWN", true).CapitalizeFirst();
            //PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref baseLetterText, ref baseLetterLabel, succubus);
            //base.SendStandardLetter(baseLetterLabel, baseLetterText, this.def.letterDef, parms, succubus, Array.Empty<NamedArgument>());
           
            return true;
        }

        private IEnumerable<Pawn> ValidVictims(Map map)
        {
            foreach (Pawn pawn in map.mapPawns.FreeColonistsAndPrisonersSpawned)
            {
                if (pawn.jobs.curDriver.asleep && xxx.need_some_sex(pawn) > 1f)
                {
                    yield return pawn;
                }
            }
            yield break;
        }

        private bool TryFindFormerFaction(out Faction formerFaction)
        {
            return Find.FactionManager.TryGetRandomNonColonyHumanlikeFaction(out formerFaction, false, true, TechLevel.Undefined, false);
        }
    }
}
