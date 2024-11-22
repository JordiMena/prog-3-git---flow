import pygame
import random

# Inicialización de Pygame
pygame.init()

# Configuración del reloj y FPS (frames por segundo)
clock = pygame.time.Clock()
fps = 60

# Dimensiones de la pantalla
screen_width = 864
screen_height = 936

# Crear la ventana del juego
screen = pygame.display.set_mode((screen_width, screen_height))
pygame.display.set_caption('Flappy Bird')

# Fuente y color para el texto
font = pygame.font.SysFont('Bauhaus 93', 60)
white = (255, 255, 255)

# Variables del juego
ground_scroll = 0
scroll_speed = 4
flying = False
game_over = False
pipe_gap = 150
pipe_frequency = 1500
last_pipe = pygame.time.get_ticks() - pipe_frequency
score = 0
best_score = 0  # Puntuación máxima guardada
pass_pipe = False  # Bandera para controlar el paso de las tuberías

# Cargar imágenes
bg = pygame.image.load('imagen/bg.png')  # Fondo de pantalla
ground_img= pygame.image.load('imagen/ground.png')  # Imagen del suelo
start_button_img = pygame.image.load('imagen/start-button.png')  # Imagen del botón de inicio
restart_button_img = pygame.image.load('imagen/restart.png')  # Imagen del botón de reinicio

# Escalar la imagen del botón de inicio
start_button_width = 200
start_button_height = 75
start_button_img = pygame.transform.scale(start_button_img, (start_button_width, start_button_height))

# Función para dibujar texto en la pantalla
def draw_text(text, font, text_col, x, y):
    img = font.render(text, True, text_col)
    screen.blit(img, (x, y))

# Función para reiniciar el juego
def reset_game():
    global pipe_frequency, score, game_over, best_score
    Pipe_group.empty()  # Elimina todas las tuberías del grupo
    flappy.rect.x = 100  # Reinicia la posición del pájaro
    flappy.rect.y = int(screen_height / 2)
    score = 0  # Reinicia la puntuación
    game_over = False  # Restablece el estado del juego
    pipe_frequency = 1500  # Restablece la frecuencia de aparición de las tuberías

    # Guardar el best_score en un archivo de texto
    with open("best_score.txt", "w") as file:
        file.write(str(best_score))

# Clase para representar al pájaro
class Bird(pygame.sprite.Sprite):
    def __init__(self, x, y):
        super().__init__()
        self.images = []
        self.index = 0 
        self.counter = 0
        for num in range(1, 4):
            img = pygame.image.load(f'imagen/bird{num}.png')
            self.images.append(img)
        self.image = self.images[self.index]
        self.rect = self.image.get_rect()
        self.rect.center = [x, y]
        self.vel = 0
        self.clicked = False

    def update(self):
        # Actualización del movimiento del pájaro
        if flying == True:
            # Gravedad
            self.vel += 0.5
            if self.vel > 8:
                self.vel = 8
            if self.rect.bottom < 768:
                self.rect.y += int(self.vel)

        # Actualización de la animación del pájaro
        if game_over == False:
            # Salto
            if pygame.mouse.get_pressed()[0] == 1 and self.clicked == False:
                self.clicked = True
                self.vel = -10
                pygame.mixer.Sound('audio/jump.wav').play()  # Reproducir sonido de salto
            if pygame.mouse.get_pressed()[0] == 0:
                self.clicked = False

            # Manejar la animación
            self.counter += 1
            flap_cooldown = 5

            if self.counter > flap_cooldown:
                self.counter = 0
                self.index += 1
                if self.index >= len(self.images):
                    self.index = 0
            self.image = self.images[self.index]

            self.image = pygame.transform.rotate(self.images[self.index], self.vel * -2)
        else:
            self.image = pygame.transform.rotate(self.images[self.index], -90)

# Clase para representar las tuberías
class Pipe(pygame.sprite.Sprite):
    def __init__(self, x, y, position):
        super().__init__()
        self.image = pygame.image.load('imagen/pipe.png')
        self.rect = self.image.get_rect()

        if position == 1:
            self.image = pygame.transform.flip(self.image, False, True)
            self.rect.bottomleft = [x, y - int(pipe_gap / 2)]
        elif position == -1:
            self.rect.topleft = [x, y + int(pipe_gap / 2)]

    def update(self):
        self.rect.x -= scroll_speed
        if self.rect.right < 0:
            self.kill()  # Elimina la tubería del grupo cuando sale de la pantalla

# Clase para representar el botón de inicio
class StartButton():
    def __init__(self, x, y, image):
        self.image = image
        self.rect = self.image.get_rect()
        self.rect.topleft = (x, y)

    def draw(self):
        action = False

        # Obtener posición del ratón
        mouse_pos = pygame.mouse.get_pos()

        # Verificar si el ratón está sobre el botón y hacer clic
        if self.rect.collidepoint(mouse_pos):
            if pygame.mouse.get_pressed()[0] == 1:
                action = True

        # Dibujar botón
        screen.blit(self.image, self.rect.topleft)

        return action

# Clase para representar el botón de reinicio
class RestartButton():
    def __init__(self, x, y, image):
        self.image = image
        self.rect = self.image.get_rect()
        self.rect.topleft = (x, y)

    def draw(self):
        action = False

        # Obtener posición del ratón
        mouse_pos = pygame.mouse.get_pos()

        # Verificar si el ratón está sobre el botón y hacer clic
        if self.rect.collidepoint(mouse_pos):
            if pygame.mouse.get_pressed()[0] == 1:
                action = True

        # Dibujar botón
        screen.blit(self.image, self.rect.topleft)

        return action


# Función para mostrar la pantalla de inicio
def show_start_screen():
    start_button = StartButton(screen_width // 2 - start_button_width // 2, screen_height // 2, start_button_img)
    start = True
    while start:
        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                pygame.quit()
                quit()
        screen.blit(bg, (0, 0))  # Dibujar el fondo escalado
        draw_text("Flappy Bird", font, white, screen_width // 2 - 150, screen_height // 3)  # Título
        start_button.draw()  # Dibujar el botón de inicio
        screen.blit(ground_img, (0, screen_height - ground_img.get_height()))  # Dibujar el suelo
        pygame.display.update()
        clock.tick(fps)
        if start_button.draw():
            start = False  # Cambiar a la pantalla de texto de inicio
    # Después de hacer clic en el botón "Start", mostrar solo el texto
    pygame.display.update()
    clock.tick(fps)

# Crear grupos de sprites para el pájaro y las tuberías
bird_group = pygame.sprite.Group()
Pipe_group = pygame.sprite.Group()

# Crear instancia del pájaro y agregarlo al grupo de sprites del pájaro
flappy = Bird(100, int(screen_height / 2))
bird_group.add(flappy)

# Cargar sonidos
pygame.mixer.init()
jump_sound = pygame.mixer.Sound('audio/jump.wav')
point_sound = pygame.mixer.Sound('audio/point.wav')
hit_sound = pygame.mixer.Sound('audio/hit.wav')
die_sound = pygame.mixer.Sound('audio/die.wav')
swooshing_sound = pygame.mixer.Sound('audio/swooshing.wav')

# Crear botón de reinicio
restart_button = RestartButton(screen_width // 2 - 50, screen_height // 2 - 100, restart_button_img)

death_sound_played = False  # Variable para controlar si el sonido de muerte ya se reprodujo

# Cargar el best_score desde el archivo de texto
try:
    with open("best_score.txt", "r") as file:
        best_score = int(file.read())
except FileNotFoundError:
    # Si el archivo no existe, se crea y se establece el best_score inicialmente en 0
    best_score = 0

# Llama a la función para mostrar la pantalla de inicio antes de iniciar el juego
show_start_screen()

# Bucle principal del juego
run = True
while run:

    clock.tick(fps)

    # Dibujar fondo
    screen.blit(bg, (0, 0))

    # Actualizar y dibujar el pájaro y las tuberías
    bird_group.draw(screen)
    bird_group.update()
    Pipe_group.draw(screen)
    
    # Dibujar el suelo
    screen.blit(ground_img, (ground_scroll, 768))

    # Verificar si el pájaro pasa por una tubería y actualizar la puntuación
    if len(Pipe_group) > 0:
        if bird_group.sprites()[0].rect.left > Pipe_group.sprites()[0].rect.left\
                and bird_group.sprites()[0].rect.right < Pipe_group.sprites()[0].rect.right\
                and pass_pipe == False:
                pass_pipe = True
        if pass_pipe == True:
            if bird_group.sprites()[0].rect.left > Pipe_group.sprites()[0].rect.right:
                score += 1
                pass_pipe = False
                point_sound.play()  # Reproducir sonido de puntos

    # Verificar colisiones entre el pájaro y las tuberías, o si el pájaro toca el suelo o la parte superior de la pantalla
    if (pygame.sprite.groupcollide(bird_group, Pipe_group, False, False) or flappy.rect.top < 0 or flappy.rect.bottom >= 768):
        game_over = True
        hit_sound.stop()  # Detener la reproducción del sonido de impacto
        hit_sound.play()  # Reproducir sonido de choque

    # Comprobar si el juego ha terminado y realizar reinicio si es necesario
    if flappy.rect.bottom >= 768 or flappy.rect.top <= 0:
        game_over = True
        flying = False
        hit_sound.stop()  # Detener la reproducción del sonido de impacto
        if not death_sound_played:  # Reproducir el sonido de muerte solo si no se ha reproducido antes
            die_sound.play()  # Reproducir sonido de derrota
            death_sound_played = True  # Establecer la bandera de reproducción de sonido de muerte a True

    # Actualizar el juego si no ha terminado
    if game_over == False and flying == True:
        # Generar nuevas tuberías
        time_now = pygame.time.get_ticks()
        if time_now - last_pipe > pipe_frequency:
            pipe_height = random.randint(-100, 100)
            bottom_pipe = Pipe(screen_width, int(screen_height / 2) + pipe_height, -1)
            top_pipe = Pipe(screen_width, int(screen_height / 2) + pipe_height, 1)
            Pipe_group.add(bottom_pipe)
            Pipe_group.add(top_pipe)
            last_pipe = time_now

        # Desplazar el suelo
        ground_scroll -= scroll_speed
        if abs(ground_scroll) > 35:
            ground_scroll = 0

        # Actualizar las tuberías
        Pipe_group.update()

    # Verificar si el juego ha terminado y mostrar botón de reinicio
    if game_over == True:
        if restart_button.draw() == True:
            reset_game()
            swooshing_sound.play()  # Reproducir sonido de transición
            death_sound_played = False  # Restablecer la bandera de reproducción de sonido de muerte a False

    # Actualizar 'best_score' si se ha superado la puntuación máxima anterior
    if score > best_score:
        best_score = score

    # Manejar eventos
    for event in pygame.event.get():
        if event.type == pygame.QUIT: 
            run = False
        if event.type == pygame.MOUSEBUTTONDOWN and flying == False and game_over == False:
            flying = True

    # Dibujar puntuación y puntuación máxima
    draw_text(f'Score: {score}', font, white, int(screen_width / 2 - 150), 20)
    draw_text(f'Best: {best_score}', font, white, int(screen_width / 2 + 150), 20)

    # Actualizar pantalla
    pygame.display.update()

# Salir de Pygame
pygame.quit()
