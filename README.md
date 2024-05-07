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

A execução do projeto pode ser feita buildando o dockerfile na raiz do repositório e depois executando a imagem gerada em um container. O serviço foi testado sendo executado direto pelo visual Studio e pela AWS.

## Testes

