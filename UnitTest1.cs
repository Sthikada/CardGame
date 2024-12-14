using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Lab3sem3;

namespace CardGame.Tests
{
    [TestClass]
    public class GameTests
    {
        [TestMethod]
        public void TestCreatureCardInitialization()
        {
            var creature = new CreatureCard("Гоблин", "Слабое существо.", 2, 3);

            Assert.AreEqual("Гоблин", creature.Name);
            Assert.AreEqual("Слабое существо.", creature.Description);
            Assert.AreEqual(2, creature.Attack);
            Assert.AreEqual(3, creature.Health);
        }

        [TestMethod]
        public void TestDamageSpellReducesHealth()
        {
            var creature = new CreatureCard("Огр", "Среднее существо.", 4, 5);
            var spell = new DamageSpell("Огненный шар", "Наносит урон.", 3);

            spell.DealDamage(creature);

            Assert.AreEqual(2, creature.Health);
        }

        [TestMethod]
        public void TestHealingSpellRestoresHealth()
        {
            var creature = new CreatureCard("Рыцарь", "Сильное существо.", 6, 7);
            creature.TakeDamage(4);

            var spell = new HealingSpell("Лечение", "Восстанавливает здоровье.", 5);
            spell.Heal(creature);

            Assert.AreEqual(8, creature.Health);
        }

        [TestMethod]
        public void TestBuffSpellIncreasesStats()
        {
            var creature = new CreatureCard("Дракон", "Очень сильное существо.", 8, 10);
            var buff = new BuffSpell("Усиление", "Увеличивает атаку и здоровье.", 2, 3);

            buff.Buff(creature);

            Assert.AreEqual(10, creature.Attack);
            Assert.AreEqual(13, creature.Health);
        }

        [TestMethod]
        public void TestSaveAndLoadGame()
        {
            var game = new Game();

            Game.Save(game);

            var loadedGame = Game.Load();

            Assert.IsNotNull(loadedGame);
            Assert.AreEqual(game.GetType(), loadedGame.GetType());
        }

        [TestMethod]
        public void TestPlayerDrawsCards()
        {
            var deck = new List<Card>
            {
                new DamageSpell("Огненный шар", "Наносит урон.", 4),
                new HealingSpell("Лечение", "Восстанавливает здоровье.", 5),
                new BuffSpell("Усиление", "Увеличивает атаку и здоровье.", 2, 3)
            };

            var player = new Player("Игрок 1");
            player.DrawCards(deck, 2);

            Assert.AreEqual(2, player.Hand.Count);
            Assert.AreEqual(1, deck.Count);
        }

        [TestMethod]
        public void TestGameInitialization()
        {
            var game = new Game();

            Assert.IsNotNull(game);
            Console.WriteLine("Игра успешно инициализирована."); // Лог для отладки.
        }

        [TestMethod]
        public void TestCreatureAttack()
        {
            var creature1 = new CreatureCard("Гоблин", "Слабое существо.", 2, 3);
            var creature2 = new CreatureCard("Огр", "Среднее существо.", 4, 5);

            creature1.AttackTarget(creature2);

            Assert.AreEqual(3, creature2.Health);
        }
    }
}
