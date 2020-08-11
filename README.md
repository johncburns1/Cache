# Cache
Implementation of a memory class and a simple HA Cache without full backing implementation.

The technique used for HA and robustness is to implement a Master/Worker model in which all writes
to the backing DB and workers are triggered through the master and then all reads are redirected to the
workers.  This makes use of efficiency and high availability through replication.  Since all reads are
handled by the worker nodes, the master nodes are more free to write and replicate new data.

Although this approach might be a bit slower than a traditional memory cache, in the case of a server failure,
we can just switch the master node to a replica and we will not incur significant performance hits.