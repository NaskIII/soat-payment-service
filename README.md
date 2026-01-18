# 💳 Fast Food - Payment Service (Fase 4)

## 📖 Visão Geral
Este microsserviço é responsável por gerenciar todo o fluxo financeiro do sistema de autoatendimento. Ele operacionaliza a cobrança de pedidos, registra solicitações de pagamento, integra-se com o processador (Mercado Pago) e atualiza o status dos pedidos de forma assíncrona.

## ✨ Evolução para Microsserviços (Fase 4)
* **Responsabilidade Única:** Microsserviço focado exclusivamente no domínio de pagamentos e integrações com gateways financeiros.
* **Qualidade e Segurança:** Monitoramento rigoroso de vulnerabilidades e code smells via SonarCloud, com foco em segurança de dados financeiros.
* **Infraestrutura como Código:** Provisionamento automatizado do ambiente no Azure Kubernetes Service (AKS) através de scripts Terraform.

## 🚀 Qualidade e Cobertura de Testes
O serviço de pagamentos superou os requisitos de qualidade da FIAP, garantindo a integridade dos fluxos críticos de cobrança.

### 📊 Painel de Qualidade (SonarCloud)
A análise técnica atual demonstra um nível de excelência com **91.3% de cobertura global** e aprovação total no Quality Gate.

* **Status:** Passed
* **Cobertura Total:** 91.3%
* **Métricas de Manutenibilidade:** Rating A

![Status SonarCloud Payment](image.png)

## 🛠️ Tecnologias Utilizadas
* **.NET 9** (Runtime)
* **PostgreSQL** (Banco de Dados SQL para transações financeiras)
* **Kubernetes** (Orquestração e Escalabilidade)
* **Terraform** (IaC para provisionamento Cloud)
* **GitHub Actions** (Pipeline de CI/CD)

## ⚙️ CI/CD e Governança
A governança do código é aplicada rigorosamente através de automações no GitHub:
1. **Validação de PR:** Execução automática de testes e análise do SonarCloud para cada Pull Request, exigindo aprovação para merge.
2. **Entrega Contínua:** Deploy automático para o ambiente AKS após o merge na branch principal, garantindo agilidade e consistência nas entregas.

## 🧑‍💻 Autor
Desenvolvido por **Raphael Nascimento** como parte integrante do Tech Challenge - Fase 4.
