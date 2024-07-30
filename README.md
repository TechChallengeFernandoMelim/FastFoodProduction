# FastFoodProduction

O repositorio FastFoodProduction tem por objetivo implementar uma Lambda Function responsável por lidar com a parte de produção de pedidos da lanchonete. Essa api recebe pedidos que já foram pagos via uma fila do SQS da AWS e para os funcionários poderem gerenciar os status dos mesmos.

## Variáveis de ambiente
Todas as variáveis de ambiente do projeto visam fazer integração com algum serviço da AWS. Explicaremos a finalidade de cada uma:

- AWS_ACCESS_KEY_DYNAMO: "Access key" da AWS. Recurso gerado no IAM para podermos nos conectar aos serviços da AWS;
- AWS_SECRET_KEY_DYNAMO: "Secret key" da AWS. Recurso gerado no IAM para podermos nos conectar aos serviços da AWS. Deve ser utilizado corretamente com seu par AWS_ACCESS_KEY_DYNAMO;
- AWS_TABLE_NAME_DYNAMO: Tablea do dynamo utilizada por este serviço para salvar os dados do pagamento.
- AWS_SQS_LOG: Url da fila de log no SQS da AWS.
- AWS_SQS_GROUP_ID_LOG: Group Id da fila de log no SQS da AWS.
- AWS_SQS_PRODUCTION: Url da fila de enviar pedidos para produção no SQS da AWS. Esse serviço usa essa fila para obter mensagens.



## Execução do projeto

Para executar com docker, basta executar o seguinte comando na pasta raiz do projeto para gerar a imagem:

``` docker build -t fast_food_production -f .\FastFoodProduction\Dockerfile . ```

Para subir o container, basta executar o seguinte comando:

``` 
docker run -e AWS_ACCESS_KEY_DYNAMO=""
-e AWS_SECRET_KEY_DYNAMO=""
-e AWS_TABLE_NAME_DYNAMO=""
-e AWS_SQS_LOG=""
-e AWS_SQS_GROUP_ID_LOG=""
-e AWS_SQS_PRODUCTION=""
-p 8081:8081 -p 8080:8080 fast_food_production
```

Observação: as variáveis de ambiente não estão com valores para não expor meu ambiente AWS. Para utilizar o serviço corretamente, é necessário definir um valor para as variáveis.

Além disso, como o projeto foi desenvolvido em .NET, também é possível executá-lo pelo Visual Studio ou com o CLI do .NET.



## Testes

Conforme foi solicitado, estou postando aqui as evidências de cobertura dos testes. A cobertura foi calculada via integração com o [SonarCloud](https://sonarcloud.io/) e pode ser vista nesse [link](https://sonarcloud.io/organizations/techchallengefernandomelim/projects). A integração com todos os repositórios poderá ser vista nesse link.

## Endpoints


Os endpoints presentes nesse projeto são:

- GET /GetNextOrder: recebe o próximo pedido de uma fila no SQS que é FIFO.
- GET /GetAllPendingOrders: lista todos os pedidos pendentes.
- PATCH /ChangeStatus/{in_store_order_id}/{newStatus}: muda o status de um pedido para "Preparing", "Ready" ou "Finished".