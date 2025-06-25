#  AI Behavior Overview

This document outlines the behavior logic of various enemy AIs used in the game.

---

##  RecklessAI

- Acts completely at random.
- Does not use any strategy or planning.
- Good for chaotic or unpredictable enemy behavior.

---

##  BasicStrategistAI

- Monitors its own health and reacts when it gets low.
- May or may not target player weaknesses if detected.
- Simple, cautious behavior — not too smart, but not totally reckless either.

---

##  PredatorAI

- Starts by attacking randomly to gather data.
- Once it finds a **player weakness** or identifies the **team's healer**, it locks onto that target.
- If the player uses a healing skill:
  - The PredatorAI will prioritize attacking the healer.
- If the PredatorAI is low on health:
  - It will try to use **healing skills**, if available.
- If the PredatorAI has **revival skills**, it will use them to bring back dead teammates.

---

*Designed for progressive difficulty and adaptive behavior based on player actions.*
