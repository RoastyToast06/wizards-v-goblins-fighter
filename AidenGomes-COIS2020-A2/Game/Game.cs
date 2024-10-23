namespace COIS2020.AidenGomes0801606.Assignments;

using System.Linq;
using Microsoft.Xna.Framework; // Needed for `Vector2`
using TrentCOIS.Tools.Visualization.EntityViz;


public class WizardFighterDX : Visualizer
{
    private const int NUM_WIZARDS = 5;
    private const int NUM_GOBLINS = 10;

    private List<CombatEntity> AllEntities { get; set; }
    private List<Goblin> Goblins { get; set;}
    private List<Wizard> Wizards  { get; set;}


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
        // Sort the list of all entities by their HP from highest to lowest
        AllEntities.Sort((a, b) => b.HP.CompareTo(a.HP));
        LogMessage("Welcome to my game! ~Aiden Gomes~ (0801606)");
    }


    protected override IEnumerable<CombatEntity> GetEntities()
    {
        return AllEntities;
    }


    protected override void Update()
    {
        foreach (CombatEntity entity in AllEntities.ToList())
        {
            List<CombatEntity> deadEntities = new();

            // Wiggle the entity around
            float dx = RNG.NextSingle() - 0.5f;
            float dy = RNG.NextSingle() - 0.5f;
            entity.Move(dx, dy);

            // Make sure they don't wiggle off the board
            entity.ClampPosition(EntityXRange, EntityYRange);

            //Entities attack if the opponent is in range, but only if it can attack at that frame
            if (entity is Goblin goblin)
            {
                //Goblins move in random directions at 0.6 units per frame
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
                        //Add a new wizard every 50 ticks after one dies
                        if (CurrentTimestamp % 50 == 0)
                        {
                            Wizard wizard = new();
                            Wizards.Add(wizard);

                            //Add the wizard to the list of all entities while keeping the list sorted
                            //Find the index of the first wizard with a higher max HP than the new wizard, then insert the new wizard before that index
                            int index = Wizards.FindIndex(w => w.HP < wizard.HP);
                            if (index == -1)
                            {
                                AllEntities.Add(wizard);
                            }
                            else
                            {
                                AllEntities.Insert(index, wizard);
                            }
                        }
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
                        wizard.Attack(gobby, (int)Math.Ceiling(wizard.SpellLevel/wizard.DistanceTo(gobby)), CurrentTimestamp);
                        gobby.PushAwayFrom(wizard, 1.50f);
                        gobby.ClampPosition(EntityXRange, EntityYRange);

                        if (gobby.HP <= 0)
                        {
                            //Add a new goblin every 15 ticks after one dies
                            deadEntities.Add(gobby);
                            if (CurrentTimestamp % 15 == 0)
                            {
                                Goblin newGoblin = new();
                                Goblins.Add(newGoblin);

                                //Add the goblin to the list of all entities while keeping the list sorted
                                //Find the index of the first goblin with a higher max HP than the new goblin, then insert the new goblin before that index
                                int index = Goblins.FindIndex(g => g.HP < newGoblin.HP);
                                if (index == -1)
                                    AllEntities.Add(newGoblin);
                                
                                else
                                    AllEntities.Insert(index, newGoblin);
                            }   
                        }
                    }
                }
                else
                {
                    // Move towards the nearest goblin
                    //Also check for null in case all goblins are dead
                    var nearestGoblin = Goblins.OrderBy(goblin => wizard.DistanceTo(goblin)).FirstOrDefault();
                    if (nearestGoblin == null) continue;
                    
                    //Wizards move towards the nearest goblin at 0.5 units per frame
                    wizard.MoveTowards(nearestGoblin, 0.5f);
                }
            }
            
            //Remove dead entities
            foreach (var deadEntity in deadEntities)
            {
                AllEntities.Remove(deadEntity);
                if (deadEntity is Goblin deadGoblin)
                {
                    Goblins.Remove(deadGoblin);
                }
                else if (deadEntity is Wizard deadWizard)
                {
                    Wizards.Remove(deadWizard);
                }
            }

            //If all wizards or goblins are dead, end the game
            if (Wizards.Count == 0 )
            {
                IsPlaying = false;
                LogMessage("All wizards are dead. Goblins win!");
            }
            else if (Goblins.Count == 0)
            {
                IsPlaying = false;
                LogMessage("All goblins are dead. Wizards win!");
            }
        }
    }
}
