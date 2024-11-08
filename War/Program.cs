using System;
using System.Collections.Generic;

namespace War
{
    internal class Program
    {
        static void Main(string[] args)
        {
            War war = new War();
            war.Work();
        }
    }

    class War
    {
        private Squad _squad1 = new Squad();
        private Squad _squad2 = new Squad();

        public void Work()
        {
            while (_squad1.SoldiersCount > 0 && _squad2.SoldiersCount > 0)
            {
                _squad1.ShowCurrentHealthSoldiers();
                Console.WriteLine();
                _squad2.ShowCurrentHealthSoldiers();
                Console.WriteLine();

                _squad1.Attack(_squad2.GetAllSoldiers());
                _squad2.Attack(_squad1.GetAllSoldiers());

                _squad1.RemoveDiedTargets();
                _squad2.RemoveDiedTargets();

                Console.ReadKey();
                Console.Clear();
            }

            ShowWinner();
        }

        public void ShowWinner()
        {
            if (_squad1.SoldiersCount > 0)
            {
                Console.WriteLine($"{_squad1.Id + 1} взвод победил");
            }
            else if (_squad2.SoldiersCount > 0)
            {
                Console.WriteLine($"{_squad2.Id + 1} взвод победил");
            }
            else
            {
                Console.WriteLine("Ничья");
            }
        }
    }

    class Soldier
    {
        public Soldier(string name, int healthPoint, int damage)
        {
            Name = name;
            HealthPoint = healthPoint;
            Damage = damage;
        }

        public string Name { get; private set; }
        public int HealthPoint { get; private set; }
        public int Damage { get; private set; }

        public virtual void Attack(List<Soldier> soldiers)
        {
            int targetIndex = UserUtils.GenerateRandomNumber(0, soldiers.Count);
            soldiers[targetIndex].TakeDamage(Damage);
        }

        public virtual void TakeDamage(int damage)
        {
            HealthPoint -= damage;
        }

        public void ShowCurrentHealth(int id)
        {
            Console.WriteLine($"{Name} здоровья: {HealthPoint}. Отряд {id + 1}");
        }
    }

    class CommonSoldier : Soldier
    {
        public CommonSoldier() : base("Обычный воин.", 100, 15) { }
    }

    class MultiDamageSoldier : Soldier
    {
        private int _multiplier = 2;

        public MultiDamageSoldier() : base("Вождь.", 80, 15) { }

        public override void Attack(List<Soldier> soldiers)
        {
            int targetIndex = UserUtils.GenerateRandomNumber(0, soldiers.Count);
            soldiers[targetIndex].TakeDamage(Damage * _multiplier);
        }
    }

    class MultiTargetSoldierNoRepeat : Soldier
    {
        public MultiTargetSoldierNoRepeat() : base("Горец.", 100, 15) { }

        public override void Attack(List<Soldier> soldiers)
        {
            int randomCount = UserUtils.GenerateRandomNumber(0, soldiers.Count);

            for (int i = 0; i < randomCount; i++)
            {
                int randomIndex = UserUtils.GenerateRandomNumber(0, UserUtils.GetAvailableIndices(soldiers).Count);
                int targetIndex = UserUtils.GenerateRandomNumber(0, soldiers.Count);

                soldiers[targetIndex].TakeDamage(Damage);
                UserUtils.GetAvailableIndices(soldiers).RemoveAt(randomIndex);
            }
        }
    }

    class MultiTargetSoldierWithRepeat : Soldier
    {
        public MultiTargetSoldierWithRepeat() : base("Валькирия.", 100, 15) { }

        public override void Attack(List<Soldier> soldiers)
        {
            int randomCount = UserUtils.GenerateRandomNumber(1, soldiers.Count);

            for (int i = 0; i < randomCount; i++)
            {
                int randomIndex = UserUtils.GenerateRandomNumber(0, UserUtils.GetAvailableIndices(soldiers).Count);
                soldiers[randomIndex].TakeDamage(Damage);
            }
        }
    }

    class Squad
    {
        private static int s_idCounter;
        private List<Soldier> _soldiers;

        public Squad()
        {
            Id = s_idCounter++;
            _soldiers = new List<Soldier>()
            {
                new CommonSoldier(),
                new MultiDamageSoldier(),
                new MultiTargetSoldierNoRepeat(),
                new MultiTargetSoldierWithRepeat()
            };
        }

        public int Id { get; private set; }
        public int SoldiersCount => _soldiers.Count;

        public void ShowCurrentHealthSoldiers()
        {
            foreach (var soldier in _soldiers)
            {
                soldier.ShowCurrentHealth(Id);
            }
        }

        public List<Soldier> GetAllSoldiers()
        {
            return new List<Soldier>(_soldiers);
        }

        public void Attack(List<Soldier> targets)
        {
            for (int i = 0; i < _soldiers.Count; i++)
            {
                _soldiers[i].Attack(targets);
            }
        }

        public void RemoveDiedTargets()
        {
            for (int i = _soldiers.Count - 1; i >= 0; i--)
            {
                if (_soldiers[i].HealthPoint <= 0)
                    _soldiers.RemoveAt(i);
            }
        }
    }

    static class UserUtils
    {
        private static Random s_random = new Random();

        public static int GenerateRandomNumber(int min, int max)
        {
            return s_random.Next(min, max);
        }

        public static List<int> GetAvailableIndices(List<Soldier> soldiers)
        {
            List<int> availableIndices = new List<int>();

            for (int i = 0; i < soldiers.Count; i++)
            {
                availableIndices.Add(i);
            }

            return availableIndices;
        }
    }
}
