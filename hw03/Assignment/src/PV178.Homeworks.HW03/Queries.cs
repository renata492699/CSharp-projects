﻿using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks.Dataflow;
using PV178.Homeworks.HW03.DataLoading.DataContext;
using PV178.Homeworks.HW03.DataLoading.Factory;
using PV178.Homeworks.HW03.Model;
using PV178.Homeworks.HW03.Model.Enums;

namespace PV178.Homeworks.HW03
{
    public class Queries
    {
        private IDataContext? _dataContext;
        public IDataContext DataContext => _dataContext ??= new DataContextFactory().CreateDataContext();

        /// <summary>
        /// Ukážkové query na pripomenutie základnej LINQ syntaxe a operátorov. Výsledky nie je nutné vracať
        /// pomocou jedného príkazu, pri zložitejších queries je vhodné si vytvoriť pomocné premenné cez `var`.
        /// Toto query nie je súčasťou hodnotenia.
        /// </summary>
        /// <returns>The query result</returns>
        public int SampleQuery()
        {
            return DataContext.Countries
                .Where(a => a.Name?[0] >= 'A' && a.Name?[0] <= 'G') 
                .Join(DataContext.SharkAttacks, 
                    country => country.Id, 
                    attack => attack.CountryId, 
                    (country, attack) => new { country, attack } 
                )
                .Join(DataContext.AttackedPeople,
                    ca => ca.attack.AttackedPersonId, 
                    person => person.Id, 
                    (ca, person) => new { ca, person } 
                )
                .Where(p => p.person.Sex == Sex.Male) 
                .Count(a => a.person.Age >= 15 && a.person.Age <= 40);
        }

        /// <summary>
        /// Úloha č. 1
        ///
        /// Vráťte zoznam, v ktorom je textová informácia o každom človeku,
        /// na ktorého v štáte Bahamas zaútočil žralok s latinským názvom začínajúcim sa 
        /// na písmeno I alebo N.
        /// 
        /// Túto informáciu uveďte v tvare:
        /// "{meno človeka} was attacked in Bahamas by {latinský názov žraloka}"
        /// </summary>
        /// <returns>The query result</returns>
        public List<string> InfoAboutPeopleThatNamesStartsWithCAndWasInBahamasQuery()
        {
            return DataContext.Countries
                .Where(country => country.Name?.ToLower() == "bahamas")
                .Join(DataContext.SharkAttacks,
                    country => country.Id,
                    attack => attack.CountryId,
                    (_, attack) => attack)
                .Join(DataContext.SharkSpecies
                        .Where(shark => shark.LatinName?[0] == 'I' || shark.LatinName?[0] == 'N'),
                    attack => attack.SharkSpeciesId,
                    shark => shark.Id,
                    (attack, shark) => new { attack.AttackedPersonId, SharkLatinName = shark.LatinName })
                .Join(DataContext.AttackedPeople,
                    attack => attack.AttackedPersonId,
                    person => person.Id,
                    (attack, person) => new
                        { attack.SharkLatinName, PersonName = person.Name })
                .Select(name => $"{name.PersonName} was attacked in Bahamas by {name.SharkLatinName}")
                .ToList();
        }

        /// <summary>
        /// Úloha č. 2
        ///
        /// Firma by chcela expandovať do krajín s nedemokratickou formou vlády – monarchie alebo teritória.
        /// Pre účely propagačnej kampane by chcela ukázať, že žraloky v týchto krajinách na ľudí neútočia
        /// s úmyslom zabiť, ale chcú sa s nimi iba hrať.
        /// 
        /// Vráťte súhrnný počet žraločích utokov, pri ktorých nebolo preukázané, že skončili fatálne.
        /// 
        /// Požadovany súčet vykonajte iba pre krajiny s vládnou formou typu 'Monarchy' alebo 'Territory'.
        /// </summary>
        /// <returns>The query result</returns>
        public int FortunateSharkAttacksSumWithinMonarchyOrTerritoryQuery()
        {
            return DataContext.Countries
                .Where(country => country.GovernmentForm == GovernmentForm.Monarchy ||
                                  country.GovernmentForm == GovernmentForm.Territory)
                .Join(DataContext.SharkAttacks
                        .Where(attack => attack.AttackSeverenity != AttackSeverenity.Fatal),
                    country => country.Id,
                    attack => attack.CountryId,
                    (country, attack) => new { country, attack })
                .Count();
        }

        /// <summary>
        /// Úloha č. 3
        ///
        /// Marketingovému oddeleniu dochádzajú nápady ako pomenovávať nové produkty.
        /// 
        /// Inšpirovať sa chcú prezývkami žralokov, ktorí majú na svedomí najviac
        /// útokov v krajinách na kontinente 'South America'. Pre pochopenie potrebujú 
        /// tieto informácie vo formáte slovníku:
        /// 
        /// (názov krajiny) -> (prezývka žraloka s najviac útokmi v danej krajine)
        /// </summary>
        /// <returns>The query result</returns>
        
        public Dictionary<string, string> MostProlificNicknamesInCountriesQuery()
        {
            var sharkAttack = DataContext.SharkSpecies
                .Where(shark => !string.IsNullOrEmpty(shark.AlsoKnownAs))
                .Join(DataContext.SharkAttacks,
                    shark => shark.Id,
                    attack => attack.SharkSpeciesId,
                    (shark, attack) => new { shark.AlsoKnownAs, attack.CountryId });
            
            return DataContext.Countries
                .Where(country => country.Continent?.ToLower() == "south america")
                .GroupJoin(sharkAttack,
                    country => country.Id,
                    attack => attack.CountryId,
                    (country, attacks) => new { CountryName = country.Name, attacks })
                .Where(countryG => countryG.attacks.Any())
                .Select(countryG => new
                {
                    countryG.CountryName,
                    SharkName = countryG.attacks
                        .GroupBy(attack => attack.AlsoKnownAs)
                        .OrderByDescending(sharkG => sharkG.Count())
                        .First().Key
                })
                .ToDictionary(name => name.CountryName ?? string.Empty,
                    name => name.SharkName ?? string.Empty);
        }
        
        /// <summary>
        /// Úloha č. 4
        ///
        /// Firma chce začať kompenzačnú kampaň a potrebuje k tomu dáta.
        ///
        /// Preto zistite, ktoré žraloky útočia najviac na mužov. 
        /// Vráťte ID prvých troch žralokov, zoradených zostupne podľa počtu útokov na mužoch.
        /// </summary>
        /// <returns>The query result</returns>
        public List<int> ThreeSharksOrderedByNumberOfAttacksOnMenQuery()
        {
            return DataContext.AttackedPeople
                .Where(person => person.Sex == Sex.Male)
                .Join(DataContext.SharkAttacks,
                    person => person.Id,
                    attack => attack.AttackedPersonId,
                    (_, attack) => attack)
                .GroupBy(attack => attack.SharkSpeciesId)
                .OrderByDescending(sharkG => sharkG.Count())
                .Take(3)
                .Select(sharkG => sharkG.Key)
                .ToList();
        }

        /// <summary>
        /// Úloha č. 5
        ///
        /// Oslovila nás medzinárodná plavecká organizácia. Chce svojich plavcov motivovať možnosťou
        /// úteku pred útokom žraloka.
        ///
        /// Potrebuje preto informácie o priemernej rýchlosti žralokov, ktorí
        /// útočili na plávajúcich ľudí (informácie o aktivite počas útoku obsahuje "Swimming" alebo "swimming").
        /// 
        /// Pozor, dáta požadajú oddeliť podľa jednotlivých kontinentov. Ignorujte útoky takých druhov žralokov,
        /// u ktorých nie je známa maximálná rýchlosť. Priemerné rýchlosti budú zaokrúhlené na dve desatinné miesta.
        /// </summary>
        /// <returns>The query result</returns>

        public Dictionary<string, double> SwimmerAttacksSharkAverageSpeedQuery()
        {
            return DataContext.SharkAttacks
                .Where(attack => attack.Activity != null && attack.Activity.ToLower().Contains("swimming"))
                .Join(DataContext.SharkSpecies
                        .Where(shark => shark.TopSpeed != null),
                    attack => attack.SharkSpeciesId,
                    shark => shark.Id,
                    (attack, shark) => new { attack.CountryId, SharkSpeed = shark.TopSpeed })
                .Join(DataContext.Countries,
                    attack => attack.CountryId,
                    country => country.Id,
                    (attack, country) => new { country.Continent, attack.SharkSpeed })
                .GroupBy(
                    attack => attack.Continent, 
                    attack => attack.SharkSpeed,
                    (key, speeds) => new
                    {
                        Continent = key ?? string.Empty, 
                        AverageSpeed = speeds.Average() ?? 0
                    })
                .ToDictionary(contAvgSpeed => contAvgSpeed.Continent, group => Math.Round(group.AverageSpeed, 2));
        }

        /// <summary>
        /// Úloha č. 6
        ///
        /// Zistite všetky nefatálne (AttackSeverenity.NonFatal) útoky spôsobené pri člnkovaní 
        /// (AttackType.Boating), ktoré mal na vine žralok s prezývkou "Zambesi shark".
        /// Do výsledku počítajte iba útoky z obdobia po 3. 3. 1960 (vrátane) a ľudí,
        /// ktorých meno začína na písmeno z intervalu <D, K> (tiež vrátane).
        /// 
        /// Výsledný zoznam mien zoraďte abecedne.
        /// </summary>
        /// <returns>The query result</returns>

        public List<string> NonFatalAttemptOfZambeziSharkOnPeopleBetweenDAndKQuery()
        {
            return DataContext.SharkAttacks
                .Where(attack => attack.Type != null && attack.Type == AttackType.Boating &&
                                 attack.DateTime != null && attack.DateTime >= new DateTime(1960, 03, 03) &&
                                 attack.AttackSeverenity != null && attack.AttackSeverenity == AttackSeverenity.NonFatal)
                .Join(DataContext.SharkSpecies.Where(shark => shark.AlsoKnownAs?.ToLower() == "zambesi shark"),
                    attack => attack.SharkSpeciesId,
                    shark => shark.Id,
                    (attack, _) => new { attack.AttackedPersonId })
                .Join(DataContext.AttackedPeople.Where(person => person.Name?[0] >= 'D' && person.Name?[0] <= 'K'),
                    attack => attack.AttackedPersonId,
                    person => person.Id,
                    (_, person) => new { person.Name })
                .Select(person => person.Name ?? string.Empty)
                .ToList();
        }

        /// <summary>
        /// Úloha č. 7
        ///
        /// Zistilo sa, že desať najľahších žralokov sa správalo veľmi podozrivo počas útokov v štátoch Južnej Ameriky.
        /// 
        /// Vráťte preto zoznam dvojíc, kde pre každý štát z Južnej Ameriky bude uvedený zoznam žralokov,
        /// ktorí v tom štáte útočili. V tomto zozname môžu figurovať len vyššie spomínaných desať najľahších žralokov.
        /// 
        /// Pokiaľ v nejakom štáte neútočil žiaden z najľahších žralokov, zoznam žralokov u takého štátu bude prázdny.
        /// </summary>
        /// <returns>The query result</returns>

        public List<Tuple<string, List<SharkSpecies>>> LightestSharksInSouthAmericaQuery()
        {
            var lightestSharksAttacks = DataContext.SharkSpecies
                .OrderBy(shark => shark.Weight)
                .Take(10)
                .Join(DataContext.SharkAttacks,
                    shark => shark.Id,
                    attack => attack.SharkSpeciesId,
                    (shark, attack) => new { shark, attack.CountryId });
            
            return DataContext.Countries
                .Where(country => country.Continent?.ToLower() == "south america")
                .GroupJoin(lightestSharksAttacks,
                    country => country.Id,
                    attack => attack.CountryId,
                    (country, attacks) => new Tuple<string, List<SharkSpecies>>
                        (
                            country.Name ?? string.Empty,
                            attacks.Select(a => a.shark).Distinct().ToList()
                        ))
                .ToList();
        }

        /// <summary>
        /// Úloha č. 8
        ///
        /// Napísať hocijaký LINQ dotaz musí byť pre Vás už triviálne. Riaditeľ firmy vás preto chce
        /// využiť na testovanie svojich šialených hypotéz.
        /// 
        /// Zistite, či každý žralok, ktorý má maximálnu rýchlosť aspoň 56 km/h zaútočil aspoň raz na
        /// človeka, ktorý mal viac ako 56 rokov. Výsledok reprezentujte ako pravdivostnú hodnotu.
        /// </summary>
        /// <returns>The query result</returns>
        public bool FiftySixMaxSpeedAndAgeQuery()
        {
            var sharkWithSpecificTopSpeed = DataContext.SharkSpecies
                .Where(shark => shark.TopSpeed != null && shark.TopSpeed >= 56)
                .Select(shark => shark.Id)
                .ToList();

            var personSpecificAgeAttack = DataContext.AttackedPeople
                .Where(person => person.Age != null && person.Age > 56)
                .Join(DataContext.SharkAttacks,
                    person => person.Id,
                    attack => attack.AttackedPersonId,
                    (_, attack) => new { attack.SharkSpeciesId });

            return sharkWithSpecificTopSpeed
                .GroupJoin(personSpecificAgeAttack,
                    shark => shark,
                    attack => attack.SharkSpeciesId,
                    (shark, attacks) => new { shark, attacks })
                .Aggregate(true, (result, next) => result && next.attacks.Any());
        }

        /// <summary>
        /// Úloha č. 9
        ///
        /// Ohromili ste svojim výkonom natoľko, že si od Vás objednali rovno textové výpisy.
        /// Samozrejme, že sa to dá zvladnúť pomocou LINQ.
        /// 
        /// Chcú, aby ste pre všetky fatálne útoky v štátoch začínajúcich na písmeno 'B' alebo 'R' urobili výpis v podobe: 
        /// "{Meno obete} was attacked in {názov štátu} by {latinský názov žraloka}"
        /// 
        /// Záznam, u ktorého obeť nemá meno
        /// (údaj neexistuje, nejde o vlastné meno začínajúce na veľké písmeno, či údaj začína číslovkou)
        /// do výsledku nezaraďujte. Získané pole zoraďte abecedne a vraťte prvých 5 viet.
        /// </summary>
        /// <returns>The query result</returns>
        public List<string> InfoAboutPeopleAndCountriesOnBorRAndFatalAttacksQuery()
        {
            return DataContext.Countries
                .Where(country => country.Name?[0] == 'B' || country.Name?[0] == 'R')
                .Join(DataContext.SharkAttacks
                        .Where(attack => attack.AttackSeverenity != null && attack.AttackSeverenity == AttackSeverenity.Fatal),
                    country => country.Id,
                    attack => attack.CountryId,
                    (country, attack) => new { CountryName = country.Name, attack.SharkSpeciesId, attack.AttackedPersonId })
                .Join(DataContext.SharkSpecies,
                    attack => attack.SharkSpeciesId,
                    shark => shark.Id,
                    (attack, shark) => new { attack.CountryName, attack.AttackedPersonId, SharkLatinName = shark.LatinName })
                .Join(DataContext.AttackedPeople.
                        Where(person => person.Name != null &&
                                        char.IsLetter(person.Name[0]) &&
                                        char.IsUpper(person.Name[0])),
                    attack => attack.AttackedPersonId,
                    person => person.Id,
                    (attack, person) => new { attack.CountryName, attack.SharkLatinName, PersonName = person.Name })
                .Select(attack =>
                    $"{attack.PersonName} was attacked in {attack.CountryName} by {attack.SharkLatinName}")
                .OrderBy(sentence => sentence)
                .Take(5)
                .ToList();
        }
        /// <summary>
        /// Úloha č. 10
        ///
        /// Nedávno vyšiel zákon, že každá krajina Európy začínajúca na písmeno A až L (vrátane)
        /// musí zaplatiť pokutu 250 jednotiek svojej meny za každý žraločí útok na ich území.
        /// Pokiaľ bol tento útok smrteľný, musia dokonca zaplatiť 300 jednotiek. Ak sa nezachovali
        /// údaje o tom, či bol daný útok smrteľný alebo nie, nemusia platiť nič.
        /// Áno, tento zákon je spravodlivý...
        /// 
        /// Vráťte informácie o výške pokuty európskych krajín začínajúcich na A až L (vrátane).
        /// Tieto informácie zoraďte zostupne podľa počtu peňazí, ktoré musia tieto krajiny zaplatiť.
        /// Vo finále vráťte 5 záznamov s najvyššou hodnotou pokuty.
        /// 
        /// V nasledujúcej sekcii môžete vidieť príklad výstupu v prípade, keby na Slovensku boli 2 smrteľné útoky,
        /// v Česku jeden fatálny + jeden nefatálny a v Maďarsku žiadny:
        /// <code>
        /// Slovakia: 600 EUR
        /// Czech Republic: 550 CZK
        /// Hungary: 0 HUF
        /// </code>
        /// 
        /// </summary>
        /// <returns>The query result</returns>
        public List<string> InfoAboutFinesInEuropeQuery()
        {
            var attackFines = DataContext.SharkAttacks
                .Where(attack => attack.AttackSeverenity != null && attack.AttackSeverenity != AttackSeverenity.Unknown)
                .Select(attack => new
                {
                    attack.CountryId,
                    Fine = attack.AttackSeverenity == AttackSeverenity.Fatal ? 300 : 250
                });

            return DataContext.Countries
                .Where(country => country.Continent?.ToLower() == "europe" && 
                                  country.Name?[0] >= 'A' && country.Name?[0] <= 'L')
                .GroupJoin(attackFines,
                    country => country.Id,
                    attack => attack.CountryId,
                    (country, attacks) => new
                    {
                        CountryName = country.Name,
                        country.CurrencyCode,
                        Fines = attacks.Sum(attack => attack.Fine)
                    })
                .OrderByDescending(country => country.Fines)
                .Take(5)
                .Select(country => $"{country.CountryName}: {country.Fines} {country.CurrencyCode}")
                .ToList();
        }

        /// <summary>
        /// Úloha č. 11
        ///
        /// Organizácia spojených žraločích národov výhlásila súťaž: 5 druhov žralokov, 
        /// ktoré sú najviac agresívne získa hodnotné ceny.
        /// 
        /// Nájdite 5 žraločích druhov, ktoré majú na svedomí najviac ľudských životov,
        /// druhy zoraďte podľa počtu obetí.
        ///
        /// Výsledok vráťte vo forme slovníku, kde
        /// kľúčom je meno žraločieho druhu a
        /// hodnotou je súhrnný počet obetí spôsobený daným druhom žraloka.
        /// </summary>
        /// <returns>The query result</returns>
        public Dictionary<string, int> FiveSharkNamesWithMostFatalitiesQuery()
        {
            return DataContext.SharkSpecies
                .GroupJoin(DataContext.SharkAttacks
                        .Where(attack => attack.AttackSeverenity != null && attack.AttackSeverenity == AttackSeverenity.Fatal),
                    shark => shark.Id,
                    attack => attack.SharkSpeciesId,
                    (shark, attacks) => new { SharkName = shark.Name, Fatalities = attacks.Count() })
                .OrderByDescending(shark => shark.Fatalities)
                .Take(5)
                .ToDictionary(shark => shark.SharkName ?? string.Empty, shark => shark.Fatalities);
        }

        /// <summary>
        /// Úloha č. 12
        ///
        /// Riaditeľ firmy chce si chce podmaňiť čo najviac krajín na svete. Chce preto zistiť,
        /// na aký druh vlády sa má zamerať, aby prebral čo najviac krajín.
        /// 
        /// Preto od Vás chce, aby ste mu pomohli zistiť, aké percentuálne zastúpenie majú jednotlivé typy vlád.
        /// Požaduje to ako jeden string:
        /// "{1. typ vlády}: {jej percentuálne zastúpenie}%, {2. typ vlády}: {jej percentuálne zastúpenie}%, ...".
        /// 
        /// Výstup je potrebný mať zoradený od najväčších percent po najmenšie,
        /// pričom percentá riaditeľ vyžaduje zaokrúhľovať na jedno desatinné miesto.
        /// Pre zlúčenie musíte podľa jeho slov použiť metódu `Aggregate`.
        /// </summary>
        /// <returns>The query result</returns>
        public string StatisticsAboutGovernmentsQuery()
        {
            var countriesCount = DataContext.Countries.Count;

            var result = DataContext.Countries
                .GroupBy(country => country.GovernmentForm, country => country.Id, 
                    (form, countries) => new { GovernmentForm = form, Count =  countries.Count()})
                .OrderByDescending(form => form.Count)
                .Aggregate("", (res, form) =>
                    res + $"{form.GovernmentForm}: {(double)form.Count / countriesCount * 100:F1}%, ");
            
            return result.Remove(result.Length - 2);
        }

        /// <summary>
        /// Úloha č. 13
        ///
        /// Firma zistila, že výrobky s tigrovaným vzorom sú veľmi populárne. Chce to preto aplikovať
        /// na svoju doménu.
        ///
        /// Nájdite informácie o ľudoch, ktorí boli obeťami útoku žraloka s menom "Tiger shark"
        /// a útok sa odohral v roku 2001.
        /// Výpis majte vo formáte:
        /// "{meno obete} was tiggered in {krajina, kde sa útok odohral}".
        /// V prípade, že chýba informácia o krajine útoku, uveďte namiesto názvu krajiny reťazec "Unknown country".
        /// V prípade, že informácie o obete vôbec neexistuje, útok ignorujte.
        ///
        /// Ale pozor! Váš nový nadriadený má panický strach z operácie `Join` alebo `GroupJoin`.
        /// Informácie musíte zistiť bez spojenia hocijakých dvoch tabuliek. Skúste sa napríklad zamyslieť,
        /// či by vám pomohla metóda `Zip`.
        /// </summary>
        /// <returns>The query result</returns>
        public List<string> TigerSharkAttackZipQuery()
        {
            var tigerSharkId = DataContext.SharkSpecies
                .First(shark => shark.Name?.ToLower() == "tiger shark")
                .Id;

            var filteredAttacks = DataContext.SharkAttacks
                .Where(attack => attack.SharkSpeciesId == tigerSharkId &&
                                 attack.DateTime != null && attack.DateTime.Value.Year == 2001 &&
                                 attack.AttackedPersonId != null)
                .ToList();

            var personIds = filteredAttacks.Select(attack => attack.AttackedPersonId).Distinct();
            var countryIds = filteredAttacks.Select(attack => attack.CountryId).Distinct();
            
            var filteredPeople = DataContext.AttackedPeople
                .Where(person => personIds.Contains(person.Id))
                .OrderBy(person => person.Id);

            var filteredCountries = DataContext.Countries
                .Where(country => countryIds.Contains(country.Id))
                .OrderByDescending(country => country.Id)
                .Select(country => country.Name)
                .Append("Unknown country");

            var countryZipAttack = filteredAttacks
                .GroupBy(attack => attack.CountryId)
                .OrderByDescending(countryG => countryG.Key)
                .Zip(filteredCountries, (countryG, countryName) => new { attacks = countryG, CountryName = countryName })
                .SelectMany(countryG => countryG.attacks, (countryG, attack) => new {  countryG.CountryName, attack.AttackedPersonId });

            return countryZipAttack
                .GroupBy(attack => attack.AttackedPersonId)
                .OrderBy(personG => personG.Key)
                .Zip(filteredPeople, (personG, person) => new { attacks = personG, PersonName = person.Name })
                .SelectMany(group => group.attacks,
                    (personG, attack) => $"{personG.PersonName} was tiggered in {attack.CountryName}")
                .ToList();
        }

        /// <summary>
        /// Úloha č. 14
        ///
        /// Vedúci oddelenia prišiel s ďalšou zaujímavou hypotézou. Myslí si, že veľkosť žraloka nejako 
        /// súvisí s jeho apetítom na ľudí.
        ///
        /// Zistite pre neho údaj, koľko percent útokov má na svedomí najväčší a koľko najmenší žralok.
        /// Veľkosť v tomto prípade uvažujeme podľa dĺžky.
        /// 
        /// Výstup vytvorte vo formáte: "{percentuálne zastúpenie najväčšieho}% vs {percentuálne zastúpenie najmenšieho}%"
        /// Percentuálne zastúpenie zaokrúhlite na jedno desatinné miesto.
        /// </summary>
        /// <returns>The query result</returns>
        public string LongestVsShortestSharkQuery()
        {
            var sharkAttacksCount = DataContext.SharkAttacks.Count;

            var longest = DataContext.SharkSpecies
                .Aggregate((longest, next) => next.Length > longest.Length ? next : longest);
            var shortest = DataContext.SharkSpecies
                .Aggregate((shortest, next) => next.Length < shortest.Length ? next : shortest);
            
            var longestCount = DataContext.SharkAttacks.Count(attack => attack.SharkSpeciesId == longest.Id);
            var shortestCount = DataContext.SharkAttacks.Count(attack => attack.SharkSpeciesId == shortest.Id);
            
            return $"{(double)longestCount / sharkAttacksCount * 100:F1}% vs {(double)shortestCount / sharkAttacksCount * 100:F1}%";
        }

        /// <summary>
        /// Úloha č. 15
        ///
        /// Na koniec vašej kariéry Vám chceme všetci poďakovať a pripomenúť Vám vašu mlčanlivosť.
        /// 
        /// Ako výstup požadujeme počet krajín, v ktorých žralok nespôsobil smrť (útok nebol fatálny).
        /// Berte do úvahy aj tie krajiny, kde žralok vôbec neútočil.
        /// </summary>
        /// <returns>The query result</returns>
        public int SafeCountriesQuery()
        {
            return DataContext.Countries
                .Except(DataContext.Countries
                        .Join(DataContext.SharkAttacks
                                .Where(attack => attack.AttackSeverenity != null && attack.AttackSeverenity == AttackSeverenity.Fatal),
                            country => country.Id,
                            attack => attack.CountryId,
                            (country, _) => country))
                .Count();
        }
    }
}
