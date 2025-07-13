The [[Graph algorithm]] applies on one or multiple graphs and accesses information stored on edges, nodes, the graph itself and other data of not specific origin meaning that are variables of the host language

The storage location of information could be either on the elements of the graph ( nodes, edges, graph) or in a separate storage per graph element that will be addressed using specific keys. 

The data one which a [[Graph Algorithm]] operates have distinct names and purpose and must be identified in a unique way even in the case that two different algorithms or instances of the same algorithm use the same name to store their data. Thus, every algorithm data parameter must be identified by its name and a context. Data parameter may also have type for their identification.

A data parameter can be access using the tuple (Context, Name, Type)