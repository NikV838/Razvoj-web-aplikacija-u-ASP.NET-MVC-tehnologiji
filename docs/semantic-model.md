# Semanticki model baze podataka - SljemeTimeAttack

Ovaj dokument opisuje semanticki model EF Core baze podataka za ASP.NET Core MVC aplikaciju **SljemeTimeAttack**. Model je definiran kroz entitete u namespaceu `SljemeTimeAttack.Models` i kontekst `SljemeTimeAttackDbContext`.

## Entiteti / tablice

- `Team`
- `Driver`
- `Car`
- `Tire`
- `Rim`
- `Suspension`
- `Run`
- `RunNote`

## Glavna svojstva

### Team
- `Id` - primarni kljuc
- `Name`
- `Country`
- `Sponsor`
- `Drivers` - kolekcija vozaca u timu

### Driver
- `Id` - primarni kljuc
- `Username`
- `Name`
- `Age`
- `YearsOfExperience`
- `TeamId` - strani kljuc prema `Team`
- `Team`
- `Email`
- `PhoneNumber`
- `CarsOwned`
- `Runs`

### Car
- `Id` - primarni kljuc
- `Make`
- `Model`
- `Horsepower`
- `WeightKg`
- `Year`
- `RegistrationNumber`
- `TireId` - strani kljuc prema `Tire`
- `WheelSetup`
- `SuspensionId` - strani kljuc prema `Suspension`
- `Suspension`

### Tire
- `Id` - primarni kljuc
- `Brand`
- `Model`
- `Type`
- `SizeInMm`
- `Dot`
- `RimId` - strani kljuc prema `Rim`
- `Rim`

### Rim
- `Id` - primarni kljuc
- `Make`
- `Model`
- `SizeInJ`
- `Material`

### Suspension
- `Id` - primarni kljuc
- `Type`
- `Brand`
- `HasFrontStrutBar`
- `HasRearStrutBar`
- `RideHeightMm`
- `IsHeightAdjustable`
- `IsStiffnessAdjustable`
- `FrontStiffness`
- `RearStiffness`

### Run
- `Id` - primarni kljuc
- `DriverId` - strani kljuc prema `Driver`
- `Driver`
- `CarId` - strani kljuc prema `Car`
- `Car`
- `Track`
- `BestTime`
- `Date`
- `Direction`
- `Weather`

### RunNote
- `Id` - primarni kljuc
- `RunId` - strani kljuc prema `Run`
- `Run`
- `Note`
- `CreatedDate`

## Odnosi izmedu entiteta

- `Team` 1-N `Driver`: jedan tim moze imati vise vozaca, a vozac pripada jednom timu preko `Driver.TeamId`.
- `Driver` 1-N `Car`: vozac moze imati vise automobila kroz kolekciju `Driver.CarsOwned`.
- `Driver` 1-N `Run`: vozac moze imati vise voznji, a svaka voznja ima `DriverId`.
- `Car` 1-N `Run`: jedan automobil se moze koristiti u vise voznji, a svaka voznja ima `CarId`.
- `Rim` 1-N `Tire`: jedan naplatak moze biti povezan s vise guma, a guma ima `RimId`.
- `Tire` 1-1/1-N prema `Car`: automobil koristi gumu preko `Car.TireId` i navigacije `WheelSetup`; ista guma se po modelu moze referencirati iz vise automobila.
- `Suspension` 1-1/1-N prema `Car`: automobil koristi ovjes preko `Car.SuspensionId`; isti ovjes se po modelu moze referencirati iz vise automobila.
- `Run` 1-N `RunNote`: jedna voznja moze imati vise biljeski, a biljeska ima `RunId`.
