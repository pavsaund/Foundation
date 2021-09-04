﻿namespace Aksio.Reflection.for_TypeExtensions
{
    public class when_asking_a_type_with_a_default_constructor_if_it_has_a_default_constructor : Specification
    {
        bool result;

        void Because() => result = typeof(TypeWithDefaultConstructor).HasDefaultConstructor();

        [Fact] void should_return_true() => result.ShouldBeTrue();
    }
}
