# FinderNET - An Advanced Discord Bot
![FinderNET](https://cdn.discordapp.com/avatars/939922948163244082/fefad3b436fb40787c958f9230a5f792.png)

FinderNET is a Discord bot written in [Discord.NET](https://github.com/discord-net/Discord.Net) in C#. It is remake of a bot called [Finder](https://github.com/FinderDiscord/Finder) written in [Discord.py](https://github.com/Rapptz/discord.py) in Python which has since been discontinued due to the [Discontinuation of Discord.py](https://gist.github.com/Rapptz/4a2f62751b9600a31a0d3c78100287f1) and the [rapidly changing Discord API](https://github.com/discord/discord-api-docs).

## Features


## Setup/Installation
</details>
<details>
<summary>Linux</summary>
<br>

**To start you need to install Dotnet 6.0.0 or higher.**

Install DotNet - [Installing Dotnet on Linux](https://docs.microsoft.com/en-us/dotnet/core/install/linux)

Install PostgreSQL - [Installing PostgreSQL on Linux](https://www.postgresql.org/download/linux/)

### Postgresql Setup

You can start the postgresql service and create the database.

```bash
$ systemctl start postgresql
or 
$ service postgresql start
```

Then create the database.
```bash
$ sudo -u postgres createdb finder
```

Then create the user.
```bash
$ sudo -u postgres createuser finder
```

Acces the postgres Shell
```bash
$ sudo -u postgres psql
```
Provide the privileges to the postgres user
```bash
$ alter user finder with encrypted password 'enter a password here';
$ grant all privileges on database finder to finder;
$ exit;
```

**You need to clone this repository.**

```bash
$ git clone https://github.com/FinderDiscord/FinderNET.git
$ cd FinderNET/FinderNET
```

### Run the bot
```bash
$ dotnet restore
$ dotnet run
```
this will create the necessary configuration files for the bot
    
### Enter details into the Configuration

Input your sensitive info into the configuration this will include
* The Bot token (Generated from https://discord.dev)
* The database infomation (ip, port, database, user, password)
* The Test Guild to register the slash commands to. (optional) (it may take up to an hour to register slash commands if this is blank)

### Run Migrations
```bash
$ dotnet tool install --global dotnet-ef
$ sudo reboot
$ cd FinderNET/FinderNET
$ dotnet ef migrations add Installation
$ dotnet ef database update
```

### Finally run the bot.
#### The database will start automaticly if not already running
```bash
$ dotnet restore
$ dotnet run
```



</details>
Have Fun Trying out FinderNET! ????

## Contributing
We would love to see your contributions to the project. Please feel free to open an [issue](https://github.com/FinderDiscord/FinderNET/issues) or [pull request](https://github.com/FinderDiscord/FinderNET/pulls)

We have a [project board](https://github.com/orgs/FinderDiscord/projects/1) where you can find the tasks that need to be completed.

We're always looking for new ways to improve our project and we appreciate any help you can give us.
