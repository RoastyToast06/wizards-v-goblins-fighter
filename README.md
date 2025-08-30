# Wizards vs. Goblins Retro Fighter

This game features an 8-bit wizard fighter against hordes of goblins!

## Project Details

This game was created as part of a Data Structures & Algorithms assignment using C# & Monogame. My teaching assistant created the renderer & sprite designs for the project; the gameplay logic and sprite implementation are implemented by me. This project utilizes ArrayLists to organize & update entity interactions. Entity logic includes:

- Goblins will attack the wizard with the lowest health; wizards will cast magic to attack all goblins within the spell's range.
- If there are no goblins in range of a wizard's spell, the wizard will move towards the closest goblin. However, goblins will move in a random direction.
- During the game, a new goblin will join the battle every 15 frames, whereas new wizards join every 50 frames.

## Gameplay Video

https://github.com/user-attachments/assets/5a52f23e-ae78-4805-8943-0901f313b01d
