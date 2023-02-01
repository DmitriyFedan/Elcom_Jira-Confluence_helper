##pip install --proxy http://fedanda:5u738mbcyopE3@192.168.0.10:8080 Jira

print ("1#####")

import discord
import os

from discord.ext import commands


Token = ""  ## put your token or read from file

##  Reading  Discord Token from  Administrationfolder
curentDirectoryPath = os.path.abspath(os.curdir)
tokenPath = curentDirectoryPath + "\Administration\DiscordBotToken.txt"

with open(tokenPath, "r") as file:
    Token = file.read()

config = {
    'token': Token,
    'prefix': 'prefix',
}

intents = discord.Intents.default() # Подключаем "Разрешения"
intents.message_content = True
# Задаём префикс и интенты
bot = commands.Bot(command_prefix='/', intents=intents )    ##  , intents=intents, proxy_auth = "http://fedanda:5u738mbcyopE4@192.168.0.10:8080" proxy = "http://fedanda:5u738mbcyopE4@192.168.0.10:8080"

print ("2#####")
# С помощью декоратора создаём первую команду

@bot.command() # Не передаём аргумент pass_context, так как он был нужен в старых версиях.
async def hallo(ctx): # Создаём функцию и передаём аргумент ctx.
    author = ctx.message.author # Объявляем переменную author и записываем туда информацию об авторе.
    print(author)

    if author.mention == "@Dmitriy_Fedan":
      await ctx.send(f'Hello, {author.mention}!') # Выводим сообщение с упоминанием автора, обращаясь к переменной author.

    else:
      await ctx.send(f'123, {author.mention}!') 


@bot.event
async def on_ready( ):
           print("Bot is ready 123")

##@bot.event
##async def hello( ):
  ##        await ctx.send("Hi")

print("3#####")
bot.run(Token)


