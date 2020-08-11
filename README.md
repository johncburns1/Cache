# Cache
Implementation of a memory cache and a simple HA cache without full backing implementation.  The logic within the caches are implemented but logic in the DBClient and CacheClient are mocked and represent "black boxes" for proof of concept.

The technique used for HA and robustness is to implement a Master/Worker model in which all writes are handled by the master and then propagated to the workers and backing DB.  All reads are handled by the workers to the
workers.  This makes use of efficiency and high availability through replication.

Although this approach might be a bit slower than a traditional memory cache, in the case of a server failure,
we can just switch the master node to a replica and we will not incur significant performance hits.