# OwlGaming V

[![.NET](https://github.com/OwlGamingCommunity/V/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/OwlGamingCommunity/V/actions/workflows/dotnet.yml)

The Grand Theft Auto V Roleplay server for RAGE:MP.

### Credits
Massive props to the developers who have spend countless hours making this project what it is right now!
- Daniels ([@DanielsGTA](https://github.com/DanielsGTA))
- Yannick ([@yannickboy15](https://github.com/yannickboy15))
- Jer ([@JeremyEspresso](https://github.com/JeremyEspresso))
- Chaos ([@braunsonm](https://github.com/braunsonm))
- Daniel ([@dlett](https://github.com/dlett))

### What is this repository for? ###

* Used for the OwlGaming Community's V RP server
* Built using C# server side and client side with a MySQL Database backend.

### References ###

- [RAGEMP Object Database](https://cdn.rage.mp/public/odb/index.html)
- [RAGEMP Natives Database](https://cdn.rage.mp/public/natives/)

### How do I get it set up? ###

#### Generating Events ####

The script uses generated RAGE events (network & UI) via JSON manifest files. These files are located in \EventDescriptors\Network\ and \EventDescriptors\UI\

Any time you add or modify an event, you must run the GenerateEvents.bat file.

#### Adding New Scripts & Resources ####

All content in OwlV is cooked into archive files which are validated during cooker time, and loaded by the server. This cook/validation step helps avoid runtime issues and makes deployment easier.

Anytime you add, remove or modify (the file name, not contents), you must update the associated JSON descriptor in \Descriptors\

Files are defined by providing their filename, and type (e.g. ClientScript, ServerScript, etc) - please refer to existing descriptors for reference.

#### Using the Cooker ####

By default, the cooker uses symbolic links to allow for fast / near-instant iteration times. In 'Debug Full Cook' and 'Release' targets, a full cook, including code analysis, archive and unarchive are performed.

OwlV requires Windows Developer Mode to be enabled. This can be enabled by going to Start -> Developer Mode -> and checking the first checkbox.

#### Build Configurations ####

* Debug x64 - Recommended for development, also enables 'fast iteration mode' for faster dev times
* Debug Full Cook x64 - Simulates a release style cook, but uses debug binaries. Only recommended for diagnosing cook issues
* Release x64 - Recommended for your server build

For running in debugger, you should start the Boot project as your startup project.

#### What variables should be I be changing? ####

You can search the code base for "TODO_GITHUB" for variables which you will want to update.

Please also refer to the environment variables below, which are required.

#### Database Configuration ####
The database connections must be configured before running the server.
Set the following environment variables:
* `GAME_DATABASE_IP`
* `GAME_DATABASE_NAME`
* `GAME_DATABASE_USERNAME`
* `GAME_DATABASE_PASSWORD`
* `GAME_DATABASE_PORT`
* `AUTH_DATABASE_IP`
* `AUTH_DATABASE_NAME`
* `AUTH_DATABASE_USERNAME`
* `AUTH_DATABASE_PASSWORD`
* `AUTH_DATABASE_PORT`

#### Web Server Configuration ####
The game server runs a web server for remote UCP calls.
The user is always `ucp`.  Calling URL is `http://IP:9001/RequestString`

By default this will only bind to 127.0.0.1. Set the environment variable below to bind to an external IP (required for LIVE server):
* `HTTP_SERVER_BIND_IP`
* `HTTP_SERVER_PASS`
* `HTTP_SERVER_WHITELIST` The IP that will be making calls

### Configuration ###

The following environment variables can be used to tweak settings:

#### Required ####
* `GAME_DATABASE_IP`
* `GAME_DATABASE_NAME`
* `GAME_DATABASE_USERNAME`
* `GAME_DATABASE_PASSWORD`
* `GAME_DATABASE_PORT`
* `AUTH_DATABASE_IP`
* `AUTH_DATABASE_NAME`
* `AUTH_DATABASE_USERNAME`
* `AUTH_DATABASE_PASSWORD`
* `AUTH_DATABASE_PORT`

#### Required for LIVE, Optional for Dev ####
| Variable | Default | Description |
|---|---|---|
| `HTTP_SERVER_BIND_IP` | `127.0.0.1` | The IP for the HTTP server to bind to |
| `HTTP_SERVER_PASS` | `0E0CECB2B6808B3BECDF28936DE54AE6481` | The authentication key for the HTTP API |
| `SENTRY_DSN` | `Disabled` | Sentry DSN |
| `DISCORD_BOT_TOKEN` | `Disabled` | Discord Bot Token |
| `DISCORD_CLIENT_ID` | `Disabled` | Discord Application Client ID |
| `ELASTICSEARCH_HOSTS` | `http://localhost:9200` | The hosts for the ElasticSearch server, delimited by spaces |
| `ELASTICSEARCH_USERNAME` | `` | The authentication username for the ElasticSearch server |
| `ELASTICSEARCH_PASSWORD` | `` | The authentication password for the ElasticSearch server |

#### Obtaining RAGE ####

You can obtain a verified working version of Rage server from the Releases page on GitHub. Please choose the highest revision and extract it into Owl -> V -> Output -> <Configuration>.

Alternatively, you can deploy any version of RAGE you wish by simply copying the server binaries from your rage server folder and copying in the C# Bridge which is found on the forums (www.rage.mp).

After copying in your binaries, you will have to compile the source code which runs the cooker, which will copy all server output into the folder above.

#### Optional ####
None yet!

### Useful things ###

#### Using Sentry ####

For more detailed docs see the [github project](https://github.com/getsentry/sentry-dotnet) or the [official docs](https://docs.sentry.io/quickstart/?platform=csharp).

Note: You may need to add the Sentry Nuget package to the resource you're working in.

**Examples**
```csharp
// Record any uncaught exceptions in a block of code
// This especially ensures that any events are sent before leaving this bloick
using (Sentry.SentrySdk.Init()) {
	// Code
}

Sentry.SentrySdk.AddBreadcrumb("Started owl_core", "Resource start");
// Code

// Use a warning level
Sentry.SentrySdk.AddBreadcrumb("Some failure", "Resource start failed", level: Sentry.Protocol.BreadcrumbLevel.Warning);

// Code

// Immediately record an event with the message and any previous breadcrumbs
Sentry.SentrySdk.CaptureMessage("Uh oh", Sentry.Protocol.SentryLevel.Warning);

// Code

// A log event that adds some information about the user before sending it
// Usually would be done in a try{}catch{}
Sentry.SentryEvent logEvent = new Sentry.SentryEvent(new Exception("Error with questions"));
logEvent.User.Username = "testing";
logEvent.User.Id = "123";
Sentry.SentrySdk.CaptureEvent(logEvent);

// Code

// Use a tag for anything in your scope
// Allows for easy searching and categorizing
Sentry.SentrySdk.CongiureScope(scope => {
	scope.SetTag("mycustomtag", "lul");
	// Change the default 'error' level to 'fatal', 'error', 'warning', 'info', or 'debug'
	scope.Level = Sentry.SentryLevel.Warning;
	// Any extra key-value pairs that aren't tags
	scope.SetExtra("character_name", "Chaos Maxime");
})

// Might be useful to also pop your scope so that you aren't adding onto another event
// https://docs.sentry.io/enriching-error-data/scopes/?platform=csharp
```

#### Discord Bot Setup ####

- Create an `app` on discord's [developer portal](https://discordapp.com/developers/applications) for the application.
- Create your bot with the following permissions
  - Change Nickname
  - Manage Nicknames
  - Manage Roles
  - Send Messages
  - Read Message History
  - Add Reactions
  - Read Messages
  - Embed Links
  - Mention Everyone
- These permissions will result in a URL like so, to add it to your server: https://discordapp.com/oauth2/authorize?client_id=<CLIENTIDHERE>&scope=bot&permissions=469977152
- Set the `DISCORD_BOT_TOKEN` environment variable
- To allow linking to discord users, set the `DISCORD_CLIENT_ID` variable.
