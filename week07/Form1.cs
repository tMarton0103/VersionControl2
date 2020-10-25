using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace week07
{
    public partial class Form1 : Form
    {
        List<Person> Population = new List<Person>();
        List<BirthProbability> BirthProbabilities = new List<BirthProbability>();
        List<DeathProbability> DeathProbabilities = new List<DeathProbability>();

        Random rng = new Random(1234);

        public Form1()
        {
            InitializeComponent();
            
            

        }

        public List<Person> GetPopulation(string csvpath)
        {
            List<Person> population = new List<Person>();

            using (StreamReader sr = new StreamReader(csvpath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');
                    population.Add(new Person()
                    {
                        BirthYear = int.Parse(line[0]),
                        Gender = (Gender)Enum.Parse(typeof(Gender), line[1]),
                        NbrOfChildren = int.Parse(line[2])
                    });
                }
            }
            return population;
        }

        public List<BirthProbability> GetBirthProbabilities(string csvpath)
        {
            List<BirthProbability> birthProbabilities = new List<BirthProbability>();

            using (StreamReader sr = new StreamReader(csvpath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');
                    birthProbabilities.Add(new BirthProbability
                    {
                        BirthYear = int.Parse(line[0]),
                        NbrOfChildren = int.Parse(line[1]),
                        BirthP = double.Parse(line[2])
                    });
                }
            }

            return birthProbabilities;
        }

        public List<DeathProbability> GetDeathProbabilities(string csvpath)
        {
            List<DeathProbability> deathProbabilities = new List<DeathProbability>();
            
            using (StreamReader sr = new StreamReader(csvpath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');
                    deathProbabilities.Add(new DeathProbability
                    {
                        Gender = (Gender)Enum.Parse(typeof(Gender), line[0]),
                        BirthYear = int.Parse(line[1]),
                        DeathP = double.Parse(line[2])
                    });
                }
            }
            return deathProbabilities;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            Simualtion();
        }

        void Simualtion()
        {
            Population = GetPopulation(@"C:\Windows\Temp\nép.csv");
            BirthProbabilities = GetBirthProbabilities(@"C:\Windows\Temp\születés.csv");
            DeathProbabilities = GetDeathProbabilities(@"C:\Windows\Temp\halál.csv");

            List<Person> Males = new List<Person>();
            List<Person> Females = new List<Person>();

            for (int y = 2005; y <= 2024; y++)
            {
                for (int i = 0; i < Population.Count; i++)
                {
                    void SimStep(int year, Person person)
                    {
                        if (!person.IsAlive) return;

                        byte age = (byte)(year - person.BirthYear);

                        double pDeath = (from x in DeathProbabilities
                                         where x.Gender == person.Gender && x.BirthYear == age
                                         select x.DeathP).FirstOrDefault();

                        if (rng.NextDouble() <= pDeath)
                        {
                            person.IsAlive = false;
                        }

                        if (person.IsAlive && person.Gender == Gender.Female)
                        {
                            double pBirth = (from x in BirthProbabilities
                                             where x.BirthYear == age
                                             select x.BirthP).FirstOrDefault();
                            if (rng.NextDouble() <= pBirth)
                            {
                                Person újszülött = new Person();
                                újszülött.BirthYear = year;
                                újszülött.NbrOfChildren = 0;
                                újszülött.Gender = (Gender)(rng.Next(1, 3));
                                Population.Add(újszülött);
                            }
                        }

                        if (person.Gender == Gender.Male)
                        {
                            Males.Add(new Person { });
                        }

                        if (person.Gender == Gender.Female)
                        {
                            Females.Add(new Person { });
                        }
                    }
                }

                int nbrOfMales = (from x in Population
                                  where x.Gender == Gender.Male && x.IsAlive
                                  select x).Count();
                int nbrOfFemales = (from x in Population
                                    where x.Gender == Gender.Female && x.IsAlive
                                    select x).Count();
                
                Console.WriteLine(string.Format("Év:{0} Fiúk:{1} Lányok:{2}", y, nbrOfMales, nbrOfFemales));
            }

        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            string result = openFileDialog1.FileName.ToString();
            textBox1.Text = result.ToString();
            
            
        }
    }

}
