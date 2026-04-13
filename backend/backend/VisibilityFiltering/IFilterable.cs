using backend.Models;

namespace backend.VisibilityFiltering;

public interface IFilterable
{
    /// <summary>
    /// A megadott lathatosagi szint szerint
    /// egy lambdat ad vissza, ami definial egy feltetelt,
    /// hogy mikor kell az adott property-t visszaadni es
    /// mikor nem.
    /// </summary>
    /// <param name="visLevel">A megadott lathatosagi szint</param>
    /// <returns>A feltetel lambda</returns>
    static abstract Func<object?, User?, bool> GetVisibilityConditionLambda(VisibilityLevel visLevel);
}