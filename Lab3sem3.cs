using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Lab3sem3
{
    // Абстрактный класс Card представляет базовую карту.
    [Serializable]
    public abstract class Card
    {
        public string Name { get; set; } // Имя карты.
        public string Description { get; set; } // Описание карты.

        // Конструктор для задания имени и описания карты.
        public Card(string name, string description)
        {
            Name = name;
            Description = description;
        }

        // Абстрактный метод Play, который реализуется в дочерних классах.
        public abstract void Play();
    }

    // Класс CreatureCard представляет существо.
    [Serializable]
    public class CreatureCard : Card
    {
        public int Attack { get; set; } // Атака существа.
        public int Health { get; set; } // Здоровье существа.

        // Конструктор для создания карты существа с параметрами атаки и здоровья.
        public CreatureCard(string name, string description, int attack, int health) : base(name, description)
        {
            Attack = attack;
            Health = health;
        }

        // Реализация метода Play для вывода информации о существе.
        public override void Play()
        {
            Console.WriteLine($"Существо {Name} (Атака: {Attack}, Здоровье: {Health}) выходит на поле.");
        }

        // Метод для нанесения урона существу.
        public void TakeDamage(int damage)
        {
            Health -= damage;
            Console.WriteLine($"{Name} получает {damage} урона. Осталось здоровья: {Health}.");
        }

        // Метод для атаки другого существа.
        public void AttackTarget(CreatureCard target)
        {
            Console.WriteLine($"{Name} атакует {target.Name} на {Attack} урона!");
            target.TakeDamage(Attack);
        }
    }

    // Абстрактный класс SpellCard представляет базовую карту заклинания.
    [Serializable]
    public abstract class SpellCard : Card
    {
        public SpellCard(string name, string description) : base(name, description) { }
    }

    // Класс HealingSpell представляет карту исцеляющего заклинания.
    [Serializable]
    public class HealingSpell : SpellCard
    {
        public int HealAmount { get; set; } // Количество здоровья, которое восстанавливает заклинание.

        public HealingSpell(string name, string description, int healAmount) : base(name, description)
        {
            HealAmount = healAmount;
        }

        // Реализация метода Play для вывода информации о заклинании.
        public override void Play()
        {
            Console.WriteLine($"Заклинание {Name} исцеляет на {HealAmount} здоровья.");
        }

        // Метод для восстановления здоровья существа.
        public void Heal(CreatureCard target)
        {
            target.Health += HealAmount;
            Console.WriteLine($"{target.Name} исцелен на {HealAmount}. Здоровье: {target.Health}.");
        }
    }

    // Класс DamageSpell представляет карту атакующего заклинания.
    [Serializable]
    public class DamageSpell : SpellCard
    {
        public int Damage { get; set; } // Количество урона, которое наносит заклинание.

        public DamageSpell(string name, string description, int damage) : base(name, description)
        {
            Damage = damage;
        }

        // Реализация метода Play для вывода информации о заклинании.
        public override void Play()
        {
            Console.WriteLine($"Заклинание {Name} наносит {Damage} урона.");
        }

        // Метод для нанесения урона существу.
        public void DealDamage(CreatureCard target)
        {
            target.TakeDamage(Damage);
        }
    }

    // Класс BuffSpell представляет карту усиливающего заклинания.
    [Serializable]
    public class BuffSpell : SpellCard
    {
        public int AttackBoost { get; set; } // Увеличение атаки.
        public int HealthBoost { get; set; } // Увеличение здоровья.

        public BuffSpell(string name, string description, int attackBoost, int healthBoost) : base(name, description)
        {
            AttackBoost = attackBoost;
            HealthBoost = healthBoost;
        }

        // Реализация метода Play для вывода информации о заклинании.
        public override void Play()
        {
            Console.WriteLine($"Заклинание {Name} улучшает существо: +{AttackBoost} к атаке и +{HealthBoost} к здоровью.");
        }

        // Метод для усиления существа.
        public void Buff(CreatureCard target)
        {
            target.Attack += AttackBoost;
            target.Health += HealthBoost;
            Console.WriteLine($"{target.Name} улучшен: Атака +{AttackBoost}, Здоровье +{HealthBoost}.");
        }
    }

    // Класс Player представляет игрока.
    [Serializable]
    public class Player
    {
        public string Name { get; set; } // Имя игрока.
        public List<Card> Hand { get; private set; } // Карты в руке игрока.
        public CreatureCard Creature { get; set; } // Существо игрока на поле.

        public Player(string name)
        {
            Name = name;
            Hand = new List<Card>();
        }

        // Метод для взятия карт из колоды.
        public void DrawCards(List<Card> deck, int numberOfCards)
        {
            for (int i = 0; i < numberOfCards; i++)
            {
                if (deck.Any())
                {
                    var card = deck.First(); // Берется первая карта из колоды.
                    deck.RemoveAt(0); // Удаляется карта из колоды.
                    Hand.Add(card); // Добавляется карта в руку игрока.
                }
            }
        }
    }

    // Класс Game представляет игровую механику.
    [Serializable]
    public class Game
    {
        private List<Card> deck; // Колода карт.
        private Player player1; // Первый игрок.
        private Player player2; // Второй игрок.
        private const string SaveFilePath = "game_state.dat"; // Путь к файлу сохранения.

        public Game()
        {
            deck = GenerateDeck(); // Генерация колоды.
            ShuffleDeck(deck); // Перемешивание колоды.

            // Создание игроков.
            player1 = new Player("Игрок 1");
            player2 = new Player("Игрок 2");

            // Выбор начальных существ для игроков.
            player1.Creature = DrawRandomCreature(deck);
            player2.Creature = DrawRandomCreature(deck);

            // Раздача карт игрокам.
            player1.DrawCards(deck, 4);
            player2.DrawCards(deck, 4);
        }

        // Метод для генерации карт в колоде.
        private List<Card> GenerateDeck()
        {
            var deck = new List<Card>();

            // Добавление карт существ.
            deck.Add(new CreatureCard("Гоблин", "Слабое существо.", 2, 3));
            deck.Add(new CreatureCard("Огр", "Среднее существо.", 4, 5));
            deck.Add(new CreatureCard("Рыцарь", "Сильное существо.", 6, 7));
            deck.Add(new CreatureCard("Дракон", "Очень сильное существо.", 8, 10));

            // Добавление карт заклинаний.
            for (int i = 0; i < 6; i++)
            {
                deck.Add(new DamageSpell($"Огненный шар {i + 1}", "Наносит урон.", 4));
            }

            for (int i = 0; i < 4; i++)
            {
                deck.Add(new HealingSpell($"Лечение {i + 1}", "Восстанавливает здоровье.", 5));
            }

            for (int i = 0; i < 4; i++)
            {
                deck.Add(new BuffSpell($"Усиление {i + 1}", "Увеличивает атаку и здоровье.", 2, 3));
            }

            return deck;
        }

        // Метод для перемешивания карт в колоде.
        private void ShuffleDeck(List<Card> deck)
        {
            var random = new Random();
            for (int i = 0; i < deck.Count; i++)
            {
                int j = random.Next(deck.Count);
                var temp = deck[i];
                deck[i] = deck[j];
                deck[j] = temp;
            }
        }

        // Метод для случайного выбора карты существа из колоды.
        private CreatureCard DrawRandomCreature(List<Card> deck)
        {
            var creature = deck.OfType<CreatureCard>().FirstOrDefault();
            if (creature != null)
            {
                deck.Remove(creature);
            }
            return creature;
        }

        // Основной игровой процесс.
        public void Play()
        {
            Console.WriteLine("Начинается игра!");
            Console.WriteLine($"{player1.Name} начинает с существом {player1.Creature.Name} (Атака: {player1.Creature.Attack}, Здоровье: {player1.Creature.Health}).");
            Console.WriteLine($"{player2.Name} начинает с существом {player2.Creature.Name} (Атака: {player2.Creature.Attack}, Здоровье: {player2.Creature.Health}).");

            var players = new[] { player1, player2 };
            int turn = 0;

            while (player1.Hand.Any() || player2.Hand.Any())
            {
                var currentPlayer = players[turn % 2];
                var opponentCreature = turn % 2 == 0 ? player2.Creature : player1.Creature;

                Console.WriteLine($"Ход {currentPlayer.Name}: Выберите карту для игры.");
                for (int i = 0; i < currentPlayer.Hand.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {currentPlayer.Hand[i].Name}: {currentPlayer.Hand[i].Description}");
                }

                int choice;
                do
                {
                    Console.WriteLine("Введите номер карты для игры:");
                } while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > currentPlayer.Hand.Count);

                var selectedCard = currentPlayer.Hand[choice - 1];
                currentPlayer.Hand.RemoveAt(choice - 1);
                selectedCard.Play();

                // Применение эффектов карты.
                if (selectedCard is DamageSpell damageSpell)
                {
                    damageSpell.DealDamage(opponentCreature);
                }
                else if (selectedCard is HealingSpell healingSpell)
                {
                    healingSpell.Heal(currentPlayer.Creature);
                }
                else if (selectedCard is BuffSpell buffSpell)
                {
                    buffSpell.Buff(currentPlayer.Creature);
                }

                turn++;
            }

            Console.WriteLine("Игра завершена!");
            Console.WriteLine(player1.Creature.Health > player2.Creature.Health
                ? "Игрок 1 побеждает!"
                : player1.Creature.Health < player2.Creature.Health
                    ? "Игрок 2 побеждает!"
                    : "Ничья!");
        }

        // Сохранение состояния игры.
        public static void Save(Game game)
        {
            using (FileStream fs = new FileStream(SaveFilePath, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, game);
                Console.WriteLine("Состояние игры сохранено.");
            }
        }

        // Загрузка сохраненной игры.
        public static Game Load()
        {
            if (File.Exists(SaveFilePath))
            {
                using (FileStream fs = new FileStream(SaveFilePath, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    Console.WriteLine("Состояние игры загружено.");
                    return (Game)formatter.Deserialize(fs);
                }
            }
            else
            {
                Console.WriteLine("Сохраненное состояние не найдено. Начинается новая игра.");
                return new Game();
            }
        }

        // Точка входа в программу.
        public static void Main(string[] args)
        {
            Console.WriteLine("Загрузить сохраненное состояние? (y/n):");
            string input = Console.ReadLine();

            Game game = input?.ToLower() == "y" ? Load() : new Game();

            game.Play();

            Console.WriteLine("Сохранить состояние игры? (y/n):");
            input = Console.ReadLine();

            if (input?.ToLower() == "y")
            {
                Save(game);
            }
        }
    }
}
