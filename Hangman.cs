using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;


namespace DOS
{
	public class Hangman
	{
		public Hangman()
		{
			Random rnd = new();
			Stopwatch clock = new();

			long score = 0, ranking1 = 0, ranking2 = 0;
			int versuche = 0, index = 0, turn = 1, durchläufe = 1, rounds = 0, runden = 1, platz = 1;
			bool repeat = false;
			string spieler1 = "", spieler2 = "", spieler = "", win = "CONGRATULATIONS!", score1 = "", score2 = "", punkte = "";
			string scoreString1 = "", scoreString2 = "";

			Console.Write("Name Spieler eins: ");
			spieler1 = Console.ReadLine();
			while (spieler1.Length > 12 || spieler1.Length <= 0 || !spieler1.All(char.IsLetter))
			{
				Console.WriteLine("Ungültiger Name.");
				Console.Write("Name Spieler eins: ");
				spieler1 = Console.ReadLine();
			}
			Console.Write("Name Spieler zwei: ");
			spieler2 = Console.ReadLine();
			while (spieler2.Length > 12 || spieler2.Length <= 0 || !spieler2.All(char.IsLetter))
			{
				Console.WriteLine("Ungültiger Name.");
				Console.Write("Name Spieler zwei: ");
				spieler2 = Console.ReadLine();
			}
			//Reihenfolge ausloten
			turn = rnd.Next(1, 2 + 1);

			do
			{
				Console.Clear();
				if (turn == 1)
				{
					spieler = spieler1;
					turn++;
				}
				else if (turn == 2)
				{
					spieler = spieler2;
					turn--;
				}

				Console.BackgroundColor = ConsoleColor.Blue;
				Console.ForegroundColor = ConsoleColor.White;
				if (rounds >= 6)
				{
					Console.Clear();
					Console.CursorVisible = false;
					Console.SetCursorPosition(50, 0);
					Console.WriteLine(win);
					score1 = ranking1.ToString();
					score2 = ranking2.ToString();
					while (score1.Length > score2.Length)
						score2 = "." + score2;
					while (score2.Length > score1.Length)
						score1 = "." + score1;
					if (ranking1 > ranking2)
					{
						punkte = "";
						for (int i = 0; i < 12 - spieler1.Length + score1.Length; i++)
						{
							punkte += ".";
						}
						platz = 1;
						scoreString1 = $"{platz}.{spieler1.ToUpper()}{punkte}{score1} Punkten";
						Console.SetCursorPosition(50, 2);
						Console.WriteLine(scoreString1);
						punkte = "";
						for (int i = 0; i < 12 - spieler2.Length + score2.Length; i++)
						{
							punkte += ".";
						}
						platz = 2;
						scoreString2 = $"{platz}.{spieler2.ToUpper()}{punkte}{score2} Punkten";
						Console.SetCursorPosition(50, 3);
						Console.WriteLine(scoreString2);
						break;
					}
					else if (ranking2 > ranking1)
					{
						punkte = "";
						for (int i = 0; i < 12 - spieler2.Length + score2.Length; i++)
						{
							punkte += ".";
						}
						platz = 1;
						scoreString2 = $"{platz}.{spieler2.ToUpper()}{punkte}{score2} Punkten";
						Console.SetCursorPosition(50, 2);
						Console.WriteLine(scoreString2);
						punkte = "";
						for (int i = 0; i < 12 - spieler1.Length + score1.Length; i++)
						{
							punkte += ".";
						}
						platz = 2;
						scoreString1 = $"{platz}.{spieler1.ToUpper()}{punkte}{score1} Punkten";
						Console.SetCursorPosition(50, 3);
						Console.WriteLine(scoreString1);
						break;
					}
					else
					{
						Console.WriteLine("Sudden death!".ToUpper());
						rounds--;
					}
				}
				if (ranking1 > ranking2 || ranking1 == ranking2)
				{
					if (ranking2 == ranking1)
					{
						Console.SetCursorPosition(Console.CursorLeft + 50, 2);
						Console.WriteLine("Gleichstand!");
					}
					Console.SetCursorPosition(Console.CursorLeft + 50, 0);
					Console.WriteLine($"Spieler 1: {spieler1} mit {ranking1} Punkten.");
					Console.SetCursorPosition(Console.CursorLeft + 50, 1);
					Console.WriteLine($"Spieler 2: {spieler2} mit {ranking2} Punkten.");
				}
				else if (ranking2 > ranking1)
				{
					Console.SetCursorPosition(Console.CursorLeft + 50, 0);
					Console.WriteLine($"Spieler 1: {spieler2} mit {ranking2} Punkten.");
					Console.SetCursorPosition(Console.CursorLeft + 50, 1);
					Console.WriteLine($"Spieler 2: {spieler1} mit {ranking1} Punkten.");
				}
				Console.ResetColor();

				Console.SetCursorPosition(0, 0);
				clock.Restart();
				if (rounds > 1 && rounds % 2 == 0)
				{
					runden++;
				}
				CreateRound(rnd, out versuche, runden, spieler, out bool lösung, out string geheimWort, out string eingabe, out string charSpeicher, out string leerWort, out string temp);

				do
				{
					Console.SetCursorPosition(0, 4);
					Console.WriteLine(leerWort);
					eingabe = Console.ReadLine().ToLower();
					if (eingabe == "cheat")
					{
						eingabe = geheimWort;
						break;
					}
					Console.SetCursorPosition(0, 5);
					Console.WriteLine(new String(' ', Console.BufferWidth));
					//füge einen neuen Buchstaben hinzu
					if (!charSpeicher.Contains(eingabe.ToUpper()) && eingabe.Length == 1 && eingabe.ToUpper().All(char.IsLetter))
					{
						charSpeicher += eingabe.ToUpper();
						charSpeicher += " ";
						Console.SetCursorPosition(50, 4);
						Console.WriteLine("Verwendete Buchstaben: " + charSpeicher);
						Console.SetCursorPosition(0, 6);
					}
					//sonst doppelte Eingabe
					else if (eingabe.Length == 1 && eingabe.All(char.IsLetter))
					{
						Console.WriteLine("Doppelte Eingabe!");
						continue;
					}
					//sonst ungültige Eingabe
					else if (eingabe != geheimWort && eingabe.Length != geheimWort.Length)
					{
						Console.WriteLine("Ungültige Eingabe!");
						continue;
					}
					//wenn Wort erraten
					if (eingabe.ToLower() == geheimWort)
					{
						Console.SetCursorPosition(0, 5);
						Console.WriteLine("RICHTIG!");
						Console.WriteLine($"Das Wort wurde nach {geheimWort.Length - versuche} Versuchen und {clock.ElapsedMilliseconds / 1000} Sekunden gelöst.");
						score = (12500 - clock.ElapsedMilliseconds / 1000 * 110) / (geheimWort.Length - versuche);
						clock.Stop();
						Console.WriteLine($"Das Geheimwort war {geheimWort.ToUpper()}.");
						Console.WriteLine($"{spieler} bekommt " + score + " Punkte.");
						lösung = true;
					}
					else if (eingabe != geheimWort && eingabe.Length > 1 || string.IsNullOrEmpty(eingabe))
					{
						Console.WriteLine("Eingabe falsch.");
					}
					//wenn Zeichen vorhanden
					else if (eingabe.Length == 1 && geheimWort.Contains(eingabe))
					{
						temp = geheimWort;
						while (geheimWort.Contains(eingabe)) //lustiges nebenfeature = eingabe(f) = wort(ffffff)
						{
							index = geheimWort.IndexOf(eingabe);
							leerWort = leerWort.Remove(index) + geheimWort.Substring(index, 1) + leerWort[(index + 1)..];
							geheimWort = geheimWort.Remove(index) + '_' + geheimWort[(index + 1)..];
						}
						Console.SetCursorPosition(0, 4);
						Console.WriteLine(leerWort);
						geheimWort = temp;

						if (geheimWort == leerWort)
						{
							Console.WriteLine($"Das Wort wurde nach {geheimWort.Length - versuche} Versuchen und {clock.ElapsedMilliseconds / 1000} Sekunden gelöst.");
							score = (10000 - clock.ElapsedMilliseconds / 1000 * 110) / (geheimWort.Length - versuche);
							clock.Stop();
							Console.WriteLine($"Das Geheimwort war {geheimWort.ToUpper()}.");
							Console.WriteLine($"{spieler} bekommt " + score + " Punkte.");
							lösung = true;
						}
					}
					else
					{
						versuche--;
						if (versuche <= 0)
						{
							Console.WriteLine($"Keine Versuche mehr. Verloren! Das Wort war {geheimWort.ToUpper()}.");
							break;
						}
						Console.WriteLine($"Noch {versuche} Versuche.");
					}
					durchläufe++;
				} while (!lösung && versuche > 0);

				if (spieler == spieler1)
				{
					ranking1 += score;
					score = 0;
				}
				else if (spieler == spieler2)
				{
					ranking2 += score;
					score = 0;
				}

				Console.WriteLine("Nächster Spieler!");
				if (ConsoleKey.E == Console.ReadKey().Key)
				{
					break;
				}
				Console.Clear();
				durchläufe = 1;
				rounds++;
			} while (!repeat);
			Console.ReadKey();
		}

		private static void CreateRound(Random rnd, out int versuche, int runden, string spieler, out bool lösung, out string geheimWort, out string eingabe, out string charSpeicher, out string leerWort, out string temp)
		{
			Console.WriteLine($"ROUND {runden}.");
			Console.WriteLine($"{spieler} ist am Zug.");
			lösung = false;
			eingabe = "";
			charSpeicher = "";
			leerWort = "";
			temp = "";
			var geheimwörter = (Geheimwörter)rnd.Next(0, 85 + 1);
			geheimWort = geheimwörter.ToString().ToLower();
			Console.WriteLine($"Das Geheimwort hat {geheimWort.Length} Buchstaben.");
			versuche = geheimWort.Length - 1;
			Console.WriteLine($"Du hast {versuche} Versuche. Viel Spass!");
			for (int i = 0; i < geheimWort.Length; i++)
			{
				leerWort += "_";
			}
		}
	}
	//PSSSST!!!!!
	enum Geheimwörter
	{
		Kanonenfutter,
		Quantenverschraenkung,
		Desoxyribonukleinsaeure,
		Dinosaurierei,
		Apfelstrudel,
		Hippopotamus,
		Kaiserpinguin,
		Dachgeschoss,
		Dopplereffekt,
		Sonnenfinsternis,
		Kungfu,
		Bananenrepublik,
		Ananas,
		Kajutenfenster,
		Autobahn,
		Lokomotive,
		Schokolade,
		Bienenhonig,
		Klosterneuburg,
		Zimtstern,
		Eishockey,
		Ausserirdischer,
		Schreibtisch,
		Schneeflocke,
		Atomreaktor,
		Sonnensystem,
		Elektrotechnik,
		Zirkuszelt,
		Radiergummi,
		Abrechnung,
		Bruder,
		Schwester,
		Lehrer,
		Schueler,
		Orangutan,
		Marmelade,
		Marzipan,
		Nilpferd,
		Nikotin,
		Alkohol,
		Krankenhaus,
		Fussball,
		Bueroklammer,
		Waeschekorb,
		Krokodil,
		Schwimmbad,
		Galionsfigur,
		Kernspintomographie,
		Zucchini,
		Gymnastik,
		Rhythmus,
		Metapher,
		Pankreas,
		Aluminium,
		Pandemie,
		Epidemie,
		Internet,
		Computer,
		Meerjungfrau,
		Saeugetier,
		Vulkan,
		Flugzeug,
		Uboot,
		Skateboard,
		Universum,
		Galaxie,
		Milchstrasse,
		Hangman,
		Programm,
		Nachrichten,
		Sommerzeit,
		Winternacht,
		Polarkreis,
		Aequator,
		Atlantik,
		Buddhismus,
		Zeremonie,
		Verbrecher,
		Schwertfisch,
		Hammerhai,
		Phantasie,
		Monarchie,
		Anarchie,
		Demokratie,
		Diktatur,
		Metastase
	}
}