namespace COIS2020.AidenGomes0801606.Assignments;

using System.Linq;
using Microsoft.Xna.Framework; // Needed for `Vector2`
using TrentCOIS.Tools.Visualization.EntityViz;


public class WizardFighterDX : Visualizer
{
    private const int NUM_WIZARDS = 5;
    private const int NUM_GOBLINS = 10;

    private List<CombatEntity> AllEntities { get; set; }
    private List<Goblin> Goblins { get; set; }
    private List<Wizard> Wizards  { get; set; }


    public WizardFighterDX()
    {
        AllEntities = new();
        Goblins = new();
        Wizards = new();

        for (int i = 0; i < NUM_WIZARDS; i++)
        {
            AllEntities.Add(new Wizard());
            Wizards.Add((Wizard)AllEntities[^1]);
        }

        for (int i = 0; i < NUM_GOBLINS; i++)
        {
            AllEntities.Add(new Goblin());
            Goblins.Add((Goblin)AllEntities[^1]);
        }

        AllEntities.Sort((a, b) => b.HP.CompareTo(a.HP));
        LogMessage("Welcome to my game! ~Aiden Gomes~");
    }


    protected override IEnumerable<CombatEntity> GetEntities() { return AllEntities; }

    protected override void Update()
    {
        List<CombatEntity> deadEntities = new();

        foreach (CombatEntity entity in AllEntities.ToList())
        {
            // Wiggle the entity around
            float dx = RNG.NextSingle() - 0.5f;
            float dy = RNG.NextSingle() - 0.5f;
            entity.Move(dx, dy);
            entity.ClampPosition(EntityXRange, EntityYRange);

            //Entities attack if the opponent is in range, but only if it can attack at that frame
            if (entity is Goblin goblin)
            {
                //Goblins move in random directions
                goblin.Move(RNG.NextSingle() - 0.5f, RNG.NextSingle() - 0.5f);

                //List all wizards in range
                var wizardsInRange = Wizards.Where(wizard => goblin.DistanceTo(wizard) <= 1 && goblin.CanAttack(CurrentTimestamp)).ToList();
                if (wizardsInRange.Count > 0)
                {
                    // Attack the weakest wizard in range
                    var weakestWizard = wizardsInRange.OrderBy(wizard => wizard.HP).FirstOrDefault();
                    if (weakestWizard == null) continue;

                    goblin.MoveTowards(weakestWizard, goblin.DistanceTo(weakestWizard) - 1.0f);
                    goblin.Attack(weakestWizard, goblin.AttackPower, CurrentTimestamp);
                    weakestWizard.PushAwayFrom(goblin, 1.50f);
                    weakestWizard.ClampPosition(EntityXRange, EntityYRange);

                    if (weakestWizard.HP <= 0)
                    {
                        deadEntities.Add(weakestWizard);
                        LogMessage($"{weakestWizard} has been defeated!");
                    }
                }
            }

            else if (entity is Wizard wizard)
            {
                //List all goblins in range
                var goblinsInRange = Goblins.Where(goblin => wizard.DistanceTo(goblin) <= 1.25 && wizard.CanAttack(CurrentTimestamp)).ToList();

                //If there are goblins in range, attack. Else, move towards the nearest goblin
                if (goblinsInRange.Count > 0)
                {
                    // Attack all goblins in range
                    foreach (var gobby in goblinsInRange)
                    {
                        wizard.Attack(gobby, (int)Math.Ceiling(wizard.SpellLevel / wizard.DistanceTo(gobby)), CurrentTimestamp);
                        gobby.PushAwayFrom(wizard, 1.50f);
                        gobby.ClampPosition(EntityXRange, EntityYRange);

                        if (gobby.HP <= 0)
                        {
                            deadEntities.Add(gobby);
                            LogMessage($"{gobby} has been slain by {wizard}!");
                        }
                    }
                }

                else
                {
                    var nearestGoblin = Goblins.OrderBy(goblin => wizard.DistanceTo(goblin)).FirstOrDefault();
                    if (nearestGoblin == null) continue;

                    //Wizards move towards the nearest goblin at 0.5 units per frame
                    wizard.MoveTowards(nearestGoblin, 0.5f);
                }
            }
        }

        // New goblins are added every 15 frames after one dies
        if (CurrentTimestamp % 15 == 0 && Goblins.Count < NUM_GOBLINS)
        {
            Goblin newGoblin = new();
            Goblins.Add(newGoblin);

            int gobIndex = AllEntities.FindIndex(g => g.MaxHP < newGoblin.MaxHP);
            if (gobIndex == -1) AllEntities.Add(newGoblin);
            else AllEntities.Insert(gobIndex, newGoblin);

            LogMessage($"A new goblin has appeared: {newGoblin}!");
        }

        // New wizards are added every 50 frames
        if (CurrentTimestamp % 50 == 0 && Wizards.Count < NUM_WIZARDS)
        {
            Wizard newWizard = new();
            Wizards.Add(newWizard);

            int wizIndex = AllEntities.FindIndex(w => w.MaxHP < newWizard.MaxHP);
            if (wizIndex == -1) AllEntities.Add(newWizard);
            else AllEntities.Insert(wizIndex, newWizard);

            LogMessage($"A new wizard has appeared: {newWizard}!");
        }

        // Remove dead entities -- distinct ensures no duplicates are removed
        foreach (var deadEntity in deadEntities.Distinct().ToList())
        {
            AllEntities.Remove(deadEntity);
            if (deadEntity is Goblin deadGoblin) Goblins.Remove(deadGoblin);
            else if (deadEntity is Wizard deadWizard) Wizards.Remove(deadWizard);
        }

        if (Wizards.Count == 0 || Goblins.Count == 0)
        {
            IsPlaying = false;
            LogMessage($"All {(Wizards.Count > 0 ? "goblins" : "wizards")} are dead. {(Wizards.Count > 0 ? "Wizards" : "Goblins")} win!");
        }
    }
}
