## Rosnet Coding Challenge
## Colin Smith 7/3/2026

## Challenge

Given a list of URLs that you supply somehow (config file, API endpoint, UI form, however you want), the system needs to handle three things end to end: 
1. A backend that checks each URL on some kind of schedule and captures, at minimum, 
HTTP status and response time. 
2. Persistence of the results so we can see history, not just the most recent state. 
3. A web UI that lets a user view that history. It doesn't have to be polished, but it does need to 
be functional enough that someone can see at a glance which URLS are healthy, which aren't, and how things have looked over time. 

## Process

# Backend

I chose to attack this back to front, so I had AI create a basic C# .NET Core application. Once the application was created and could build, I started with creating a one off query to check the status (http response status and response time) of a given set of URLs. Once that was working I created a background worker process to process a harcoded list of URLs and store the results in a SQLite database in disk.

# Frontend

With the backend functioning at a base level, I started on the frontend. I had AI create a basic react application that could fetch data from my backend locally and display the results in tables. After the basics were complete I create a line graph that displays the response time of each url over time.

# Minor Improvement

It was annoying to need to execute multiple scripts across both the frontend and backend so I set up a docker compose build that builds and starts both the frontend and the backend with 1 simple command.

# Testing

With the basics of the challenge completed, I moved on to tests. Normally with there being frontend changes, I would create e2e tests but I was short on time and needed a quick win so I had AI create a unit testing framework and create some test for the backend. The initial tests were not comprehensive so I argued with AI to make some more/better tests for a bit then hit my 2.3 hour mark and started writing this.

# Improvements

If I had more time to attack this, there are several improvements I would make
- Alerting: This is a perfect situation to set up a New Relic synthetic monitor to request the current health and alert on any issues. This would also easily allow for a synthetic monitor to be set up to monitor the history endpoint and send warnings on any urls that are misbehaving (longer response time over time, not returning 200s more frequently than one would expect, etc).
- CI/CD: You can see in my repo that there is a .github folder. This is because I originally intended to create a CI/CD process for this app, but then realized I do not currently have anywhere to host these applications from and setting that up would be spending my precious time.
- e3e testing: As mentioned in the testing section above, I would have loved to create e2e tests for this as there is frontend changes, but I ran out of time.
- Account based monitoring: I think it would be a good idea to create a way for users to register a set of urls to be monitored over time. This ties in nicely for a multi account system where the would be an auth system the frontend uses to have users log in and see data specific to their account. The ui would show a list of urls currently being monitored and allow users to update the list of urls for their account. This would at a minimum double the size of the application, so there was never going to be time for that.
- Per URL settings: There is currently no way to handle specific URLs having specific settings. This could be timeout settings, retries, etc.
- Color: The frontend is a pretty bland design that really needs polished.6