[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/2QC0Bpz-)
# CSCI 1260 — Project

## Project Instructions
All project requirements, grading criteria, and submission details are provided on **D2L**.  
Refer to D2L as the *authoritative source* for this assignment.

This repository is intentionally minimal. You are responsible for:
- Creating the solution and projects
- Designing the class structure
- Implementing the required functionality

---

To build and run the game enter "dotnet build" in the terminal in the same directory as this README file

Next, enter "dotnet run --project src/AdventureGame.Console" 

The game will now start in the terminal


To move in the game, you use wasd
w - up
s - down
a - left
d - right

You will automatically interact with things on the maze

The maze will appear in a 15x15 layout and underneath it is your health and damage

HP: 100         Damage: 10 (10 + 0)
^                       ^   ^    ^
Current health          Total damage (base damage + weapon modifier)


To win the game, you need to reach the exit tile, noted as a character "E". As long as you reach it you win.
Once you win the game you will be prompted to replay. Enter "Y" to continue or anything else to quit.

When you enter a M tile, a monster fight will occur where you will fight until one of you dies.

The UML DIAGRAM:

