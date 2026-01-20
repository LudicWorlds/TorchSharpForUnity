using System;


public struct IrisData
{
    public int Id;
    public float SepalLength;
    public float SepalWidth;
    public float PetalLength;
    public float PetalWidth;
    public string Species;
    public int SpeciesId;

    public IrisData(int id, float sepalLength, float sepalWidth, float petalLength, float petalWidth, string species)
    {
        Id = id;
        SepalLength = sepalLength;
        SepalWidth = sepalWidth;
        PetalLength = petalLength;
        PetalWidth = petalWidth;
        Species = species;
        SpeciesId = GetSpeciesId(species);
    }

    public static int GetSpeciesId(string species)
    {
        switch (species.Trim().ToLower())
        {
            case "iris-setosa":
                return 0;
            case "iris-versicolor":
                return 1;
            case "iris-virginica":
                return 2;
            default:
                throw new ArgumentException("Unknown species: " + species);
        }
    }

    public override string ToString()
    {
        return $"Id: {Id}, SepalLength: {SepalLength}, SepalWidth: {SepalWidth}, PetalLength: {PetalLength}, PetalWidth: {PetalWidth}, Species: {Species}, SpeciesId: {SpeciesId}";
    }
}

