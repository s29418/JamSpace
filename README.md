<div align="center">
<h1>
JamSpace - Music Collaboration Platform
</h1>
</div>

<br/>

<div align="center">
<img src="assets/logo.png" width="300" >
</div>

<br/>

<div align="center">
<strong>
ASP.NET Core • EF Core • Clean Architecture • React • MSSQL (Docker) • Azure Blob Storage • JWT
</strong>
</div>

<br/>
<br/>

---

## Overview
**JamSpace** is a web platform designed to help musicians collaborate remotely by combining social features with project and team management tools. The system enables artists to find collaborators, exchange ideas, manage workflows, and develop music projects within one unified environment.

The platform bridges market gaps identified during analysis, such as the lack of tools combining portfolio presentation, collaboration, audio file sharing, and structured team management.

This project was fully designed and developed by me, including system architecture, domain modeling, backend implementation, frontend application, and infrastructure setup. The entire platform was built from scratch as part of my engineering thesis.

**Note:**
Due to engineering thesis requirements, the full implementation cannot be publicly published. However, the complete project can be shared securely upon request as part of a recruitment process.

---

## Implemented Features

### Authentication & User Management
- Registration and login (JWT-based)
- Password hashing and verification
- Change password
- Change email
- Update display name
- Secure authorization using JWT claims and role checks

<details>
  <summary>Screenshots</summary>

  <img src="screenshots/register.png" width="500"/>
  <img src="screenshots/login.png" width="500"/>

</details>

### User Profile
- Viewing user profiles
- Editing profile fields:
  - bio
  - location
  - musical skills
  - preferred genres
  - uploading and updating profile picture


<img src="screenshots/profile.png">
<details>
  <summary>More screenshots</summary>

  <img src="screenshots/profile-edit.png" width="500"/>
  <img src="screenshots/profile-edit-skills.png" width="500"/>
  <img src="screenshots/profile-edit-genres.png" width="500"/>
  <img src="screenshots/profile-edit-account-unlocked.png" width="500"/>

</details>

### Teams
- Create a team
- View all teams the user belongs to
- Upload team picture to **Azure Blob Storage**
- View team details with the user's role included
- Access control via `IsUserInTeam`

<img src="screenshots/teams.png">
<details>
  <summary>More screenshots</summary>

  <img src="screenshots/teams-creating&invites.gif"/>

  <img src="screenshots/team-settings-teamname.png" width="500"/>
  <img src="screenshots/team-settings-teampicture.png" width="500"/>
  <img src="screenshots/team-settings-userrole.png" width="500"/>

</details>

### Team Invitations
- Send invitation to a user
- Accept invitation
- Reject invitation
- View all pending invitations
- Enum-based invitation status
- Automatic assignment of team role after accepting

### Infrastructure & Architecture
- Clean Architecture structure (Domain / Application / Infrastructure / API)
- EF Core Code First with migrations
- Domain entities: User, Team, TeamMember, TeamInvite, Skill, UserSkill, Genre, UserGenre, UserFollows
- Repository pattern with DI
- Azure Blob Storage integration
- MSSQL running in Docker

---

## Planned Features

### Feed & Social Features
- Posts (text, photos, shared tracks)
- Music-only feed (portfolio view)
- Likes and shares

### External Integrations
- Connect Spotify profile
- Connect SoundCloud profile
- Import tracks into user portfolio

### Project Workspace
- Audio player with time-based annotations
- General project notes
- Track version comparison
- Mentioning team members in notes

### Notifications
- New messages
- Event reminders
- Mentions

---

## Technology Stack

### **Backend**
- ASP.NET Core Web API
- Entity Framework Core (Code First)
- Clean Architecture
- CQRS pattern with MediatR
- MSSQL (Docker container)
- JWT authentication
- Azure Blob Storage for images/audio

### **Frontend**
- React with TypeScript
- Custom CSS (no UI framework)

---

## Additional Technical Information

- Complete **system design authored from scratch**, including domain model, entity relationships and system architecture

- Full modeling of business rules and data structures(teams, members, invites, user profiles, skills, genres, followers, etc.) translated into a clean and scalable domain-driven schema

- Fully structured **Clean Architecture** with strict separation of concerns(Domain -> Application -> Infrastructure -> API)

- Consistent use of **CQRS** with MediatR for all commands and queries

- Centralized **global exception handling** with unified error responses

- Authorization implemented through **JWT claims** with custom role checks

- Rich domain model with value objects, navigations and many-to-many relations (`UserSkill`, `UserGenre`, `UserFollows`)

- Usage of **Azure Blob Storage** for team images and profile pictures

- Containerized database environment using **SQL Server in Docker** for development setup

- Frontend built with TypeScript and modular structure (services, hooks, components)

- Manual and automated testing approach:
  - API testing via Swagger and Postman
  - Unit tests for business logic where applicable
