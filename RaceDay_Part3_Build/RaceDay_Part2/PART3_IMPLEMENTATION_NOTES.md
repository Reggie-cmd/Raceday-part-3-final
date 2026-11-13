# Part 3 implementation and verification checklist

The MVC project and API-side Azure upload foundation have been added. Before assessment, complete and verify these items:

1. Configure SQL Server and Azure Blob Storage credentials locally; never commit secrets.
2. Generate and apply the EF Core migration for `Event.BannerImageUrl` and `UserProfile.ProfilePictureUrl`.
3. Run API and MVC over HTTPS locally and confirm the `.RaceDay.Auth` cookie is forwarded correctly from MVC session to the API.
4. Test Participant and Organiser registration separately. Organiser registration must use the organiser code configured by the API.
5. Test unauthenticated redirects and role restrictions on every protected MVC action.
6. Verify event creation, editing, deletion, category creation, participant enrolment, enrolment listing, result recording and personal results.
7. Test JPG, PNG and WEBP image uploads, invalid MIME types, empty files, and files larger than 5 MB.
8. Confirm uploaded blob URLs load on the public event details page and profile page.
9. Confirm Azure container public-access policy matches the intended image visibility. For private storage, replace public URLs with a controlled delivery/SAS design.
10. Build the MVC Docker image and run it while the API is reachable from the container.
11. Run `dotnet restore`, `dotnet build -c Release`, and `dotnet test` from the solution root.
12. Verify the GitHub Actions run is green before recording the video.

## Suggested meaningful commit sequence

Create commits as you complete and test each item; do not create empty commits merely to reach the minimum.

1. Add MVC project to solution
2. Configure MVC session and API HttpClient
3. Add API cookie forwarding in ApiClient
4. Add registration and login views
5. Add session logout and role-aware navigation
6. Add public event listing and details
7. Add organiser dashboard
8. Add organiser event creation
9. Add organiser event editing and deletion
10. Add event category management
11. Add participant enrolment flow
12. Add organiser enrolment listing
13. Add organiser result entry
14. Add participant results page
15. Add participant profile editing
16. Add Azure Blob Storage service to API
17. Add event banner upload endpoint
18. Add participant profile picture upload endpoint
19. Add MVC image upload forms and display
20. Add MVC Dockerfile and ignore rules
21. Extend GitHub Actions to build and test solution
22. Document local setup, Azure configuration and Docker commands
23. Verify and fix authentication/session integration
24. Verify Docker and CI pipeline
