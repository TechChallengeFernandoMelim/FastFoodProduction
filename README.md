# FastFoodProduction

O repositorio FastFoodProduction tem por objetivo implementar uma Lambda Function respons�vel por lidar com a parte de produ��o de pedidos da lanchonete. Essa api recebe pedidos que j� foram pagos via uma fila do SQS da AWS e para os funcion�rios poderem gerenciar os status dos mesmos.

## Vari�veis de ambiente
Todas as vari�veis de ambiente do projeto visam fazer integra��o com algum servi�o da AWS. Explicaremos a finalidade de cada uma:

- AWS_ACCESS_KEY_DYNAMO: "Access key" da AWS. Recurso gerado no IAM para podermos nos conectar aos servi�os da AWS;
- AWS_SECRET_KEY_DYNAMO: "Secret key" da AWS. Recurso gerado no IAM para podermos nos conectar aos servi�os da AWS. Deve ser utilizado corretamente com seu par AWS_ACCESS_KEY_DYNAMO;
- AWS_TABLE_NAME_DYNAMO: Tablea do dynamo utilizada por este servi�o para salvar os dados do pagamento.
- AWS_SQS_LOG: Url da fila de log no SQS da AWS.
- AWS_SQS_GROUP_ID_LOG: Group Id da fila de log no SQS da AWS.
- AWS_SQS_PRODUCTION: Url da fila de enviar pedidos para produ��o no SQS da AWS. Esse servi�o usa essa fila para obter mensagens.



## Execu��o do projeto

A execu��o do projeto pode ser feita buildando o dockerfile na raiz do reposit�rio e depois executando a imagem gerada em um container. O servi�o foi testado sendo executado direto pelo visual Studio e pela AWS.

## Testes

Conforme foi solicitado, estou postando aqui as evid�ncias de cobertura dos testes. A cobertura foi calculada via integra��o com o [SonarCloud](https://sonarcloud.io/) e pode ser vista nesse [link](https://sonarcloud.io/organizations/techchallengefernandomelim/projects). A integra��o com todos os reposit�rios poder� ser vista nesse link.

![Coverage1](./images/coverage1.png)

![Coverage2](./images/coverage2.png)

Atrav�s das imagens � poss�vel observar que a cobertura por testes unit�rios ficou superior a 80%, conforme solicitado.

## Endpoints


Os endpoints presentes nesse projeto s�o:

- GET /GetNextOrder: recebe o pr�ximo pedido de uma fila no SQS que � FIFO.
- GET /GetAllPendingOrders: lista todos os pedidos pendentes.
- PATCH /ChangeStatus/{in_store_order_id}/{newStatus}: muda o status de um pedido para "Preparing", "Ready" ou "Finished".