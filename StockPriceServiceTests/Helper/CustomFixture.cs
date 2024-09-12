using AutoFixture;

namespace StockPriceServiceTests.Helper;

public class CustomFixture : Fixture
{
    public CustomFixture()
    {
        // Remove recursion behavior and add omit recursion behavior
        var behaviorsToRemove = Behaviors.OfType<ThrowingRecursionBehavior>().ToList();
        foreach (var behavior in behaviorsToRemove)
        {
            Behaviors.Remove(behavior);
        }
        Behaviors.Add(new OmitOnRecursionBehavior());
    }
}
