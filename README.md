# GameOfLife

Esta é uma implementação do [Jogo da Vida de Conway](https://pt.wikipedia.org/wiki/Jogo_da_vida).
Este jogo é uma simulação/jogo sem jogadores, mas o editor permite que o usuário defina as condições e aprecie o resultado.
Basicamente ele consiste numa matriz onde cada quadrado pode ser ocupado por um ser vivo genérico a escolha do usuário. A cada geração o mapa muda com base nas seguintes regras:
- Um individuo adjacente a 0 ou 1 individuos morre por solidão.
- Um individuo adjacente a 2 ou 3 individuos permanece vivo.
- Um individuo adjacente a mais de 3 individuos morre por superpopulação.
- Uma casa sem vida, adjacente de exatamente 3 individuos ganha vida, sendo tomado por um novo individuo.
As regras são simples, mas os resultados podem ser fantásticos.

Ele é tão complexo que existe uma [wiki](https://www.conwaylife.com/wiki/Main_Page) só sobre o jogo.

Para executar o simulador, você precisa do [.NET 5 rc (Run apps-Runtime)](https://dotnet.microsoft.com/download/dotnet/5.0).

Você pode baixar uma release ou baixar o launcher. O segundo irá atualizar o jogo toda vez que existir uma nova versão.

Ao entrar no jogo, clique a letra "i" para obter instruções e comandos. Além de zoom, possibilidade de copiar estruturas, o editor também permite escolher velocidade e mostra um gráfico da população ao longo do tempo.

Não é uma aplicação perfeita, e provavelmente nunca será, então ela esta sujeita a falhas.

Bom divertimento :)

## Launcher

- [1.0](https://github.com/trevisharp/GameOfLife/releases/tag/l1.0)

## Releases

- [1.0](https://github.com/trevisharp/GameOfLife/releases/tag/1%2C0)

## Features futuras (nem todas funcionam 100%)

- [x] Grade em escuro para seções brancas 
- [x] Copiar/Colar
- [x] Zoom
- [ ] Pintar estruturas
- [ ] Gerar gifs
- [x] Atualizar com base no Git Hub
- [x] Inicializar grade com tamanho melhor
- [ ] Salvar/Importar/Exportar
- [ ] Sistema de bordas conectas
- [x] Gráfico de população ao longo do tempo
- [x] Laucher
- [ ] Ferramentas de desenho