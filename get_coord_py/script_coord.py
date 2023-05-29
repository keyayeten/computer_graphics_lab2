INPUT_FILE = "tinker2.obj"

with open(INPUT_FILE) as f:
    vertices = []
    faces = []

    for line in f.readlines():
        if line.startswith("v "):
            vertex = list(map(lambda x: int(float(x))//4, line.split()[1:]))
            vertices.append(vertex)
        elif line.startswith("f "):
            face = list(map(int, line.split()[1:]))
            faces.append(face)


adjacency_list = [[] for _ in range(len(vertices))]
for face in faces:
    for i in range(3):
        v1 = face[i] - 1
        v2 = face[(i+1)%3] - 1

        # Находим координаты двух точек
        x1, y1, z1 = vertices[v1]
        x2, y2, z2 = vertices[v2]

        # Проверяем, лежат ли точки на одной прямой, параллельной одной из трех осей координат
        if (x1 == x2 and y1 == y2) or (x1 == x2 and z1 == z2) or (y1 == y2 and z1 == z2):
            # Если да, то добавляем друг к другу как соседей
            adjacency_list[v1].append(v2)
            adjacency_list[v2].append(v1)

# Исключаем дублирование вершин в adjacency_list
adjacency_list = [list(set(x)) for x in adjacency_list]

res = []
for i, vertex in enumerate(vertices):
    connected_vertices = adjacency_list[i]
    res.append([vertex, connected_vertices])

if INPUT_FILE == "tinker1.obj":
    location = 'roof'
    output = 'res1.txt'
elif INPUT_FILE == "tinker2.obj":
    location = 'floor'
    output = 'res2.txt'
elif INPUT_FILE == "tinker.obj":
    location = 'col1'
    output = 'res3.txt'

br_o, br_c = '{', '}'
with open(output, 'w') as f:
    for xyz, gr in res:
        stroka1 = f'{location}.vertices.Add({xyz[0]}, {xyz[1]}, {xyz[2]}, 1);\n'
        grane = ','.join(list(map(str, gr)))
        stroka2 = f'{location}.edges.Add(new int[] {br_o}{grane}{br_c});\n'
        f.write(stroka1)
        f.write(stroka2)
