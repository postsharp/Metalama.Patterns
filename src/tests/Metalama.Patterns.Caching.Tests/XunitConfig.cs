using Xunit;

/*
 * Configuration of the Xunit behaviors, such as tests parallelization.
 * See https://xunit.net/docs/running-tests-in-parallel
 */

[assembly: CollectionBehavior( DisableTestParallelization = true )]
