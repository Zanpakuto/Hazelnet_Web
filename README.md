## HazelNet Web

A modern flashcard and space-repetition study web application build with .NET and Blazor Interactive Server.

This project is a continuation and re-imagining of HazelNet, which originally started as cross-platoform desktop application build using AvaloniaUI (unfinished).

The current iteration transitions HazelNet into a fully web-based experience, leveraging Blazor for improved accessibility, scalability, and maintainability.

Previous Version / Iteration:
[HazelNet](https://github.com/Xhyther/HazelNet/)

---

### System Architecture 
 - **Flashcard Management Subsystem**: Handles the creating, editing, organization and categorization of flashcards into decks. Supports multimedia content and import/export functionality.
 - **Study Subsystem**: Manages active study sessions, presents flashcards for review, handles user responses, and provides real-time progress tracking during study sessions.
 - **Predictive Performance Subsystem**: Implements the FSRS (Free Spaced Repetition Scheduler) algorithm to optimize review scheduling. Predicts recall probability, adjusts review intervals based on performance, and personalizes the learning experience for each user.
 - **Analytics Subsystem**: Collects and analyzes study data to generate insights on review history, study time patterns, recall accuracy trends, and learning progress across different subjects and time periods.

---

### Software Architecture

There are three seperate but connected projects in this repo/solution(.sln). This uses or take inspiration from **Clean Architecure** with ***vertical slice architecture*** in features.
#### Domain
  - This part should only contain "***Pure***" buisness logic and classes.
  - There should be No database dependencies and UI Logic.
  - Fully testable in Isolation

**Example**:
    
    Entities / Models: Card.cs, Deck.cs, ReviewLog.cs
    FSRS Logic: FSRS Algorith Class / Inteface. Also, The mathematical formulas for updating stability / difficulty.
    Enums: Rating.cs, CardState.cs
        
#### Infrastracture
  - This part should only contain anything that talks to the outside world (Database, File System).

**Example**:
    
    Entity Framework: AppDbContext.cs, Migrations/.
    Database Configuration: DbConfig classes.

#### Web
  - The main part of the web app and composition root.
  - All Blazor components and web related technologies such as the frontend, backend and anything in-between.
  - Blazor UI components
  - Application services and orchestration
  - Request handling and state management
  - Authentication and authorization (planned)
  - Wiring Domain and Infrastructure together

---


