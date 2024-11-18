from flask import Flask, request, jsonify
import mysql.connector
from werkzeug.security import generate_password_hash, check_password_hash # Для хеширования паролей
import hashlib # Альтернатива werkzeug

app = Flask(__name__)

# Конфигурация подключения к MySQL (ЗАМЕНИТЕ НА ВАШИ ДАННЫЕ)
mydb = mysql.connector.connect(
  host="localhost",
  user="ваш_пользователь",
  password="ваш_пароль",
  database="ваша_база_данных"
)

# Функция для безопасного выполнения запросов к БД
def execute_query(query, params=None):
    cursor = mydb.cursor()
    try:
        cursor.execute(query, params)
        mydb.commit()
        return cursor
    except mysql.connector.Error as err:
        mydb.rollback()  # Отмена транзакции при ошибке
        print(f"Database error: {err}")
        return None
    finally:
        cursor.close()


# --- Users ---

@app.route('/users', methods=['GET'])
def get_users():
    cursor = execute_query("SELECT user_id, username, email FROM users")
    if cursor:
        users = cursor.fetchall()
        return jsonify([{'user_id': row[0], 'username': row[1], 'email': row[2]} for row in users])
    return jsonify({'message': 'Database error'}), 500


@app.route('/users', methods=['POST'])
def create_user():
    data = request.get_json()
    # Валидация данных (простейший пример)
    if not data.get('username') or not data.get('email') or not data.get('password'):
        return jsonify({'message': 'Missing username, email, or password'}), 400

    hashed_password = generate_password_hash(data['password']) # Хеширование пароля

    cursor = execute_query("INSERT INTO users (username, email, password_hash) VALUES (%s, %s, %s)", (data['username'], data['email'], hashed_password))
    if cursor:
        return jsonify({'message': 'User created'}), 201
    return jsonify({'message': 'Database error'}), 500



@app.route('/users/<int:user_id>', methods=['GET'])
def get_user(user_id):
    cursor = execute_query("SELECT user_id, username, email FROM users WHERE user_id = %s", (user_id,))
    if cursor:
        user = cursor.fetchone()
        if user:
            return jsonify({'user_id': user[0], 'username': user[1], 'email': user[2]})
        return jsonify({'message': 'User not found'}), 404
    return jsonify({'message': 'Database error'}), 500


@app.route('/users/<int:user_id>', methods=['PUT'])
def update_user(user_id):
    data = request.get_json()
    # Валидация данных
    if not data.get('username') or not data.get('email'):
        return jsonify({'message': 'Missing username or email'}), 400

    cursor = execute_query("UPDATE users SET username = %s, email = %s WHERE user_id = %s", (data['username'], data['email'], user_id))
    if cursor:
      if cursor.rowcount == 0:
        return jsonify({'message': 'User not found'}), 404
      return jsonify({'message': 'User updated'})
    return jsonify({'message': 'Database error'}), 500

@app.route('/users/<int:user_id>', methods=['DELETE'])
def delete_user(user_id):
    cursor = execute_query("DELETE FROM users WHERE user_id = %s", (user_id,))
    if cursor:
      if cursor.rowcount == 0:
        return jsonify({'message': 'User not found'}), 404
      return jsonify({'message': 'User deleted'})
    return jsonify({'message': 'Database error'}), 500


# --- Tests ---
@app.route('/tests', methods=['GET'])
def get_tests():
    cursor = execute_query("SELECT test_id, test_name, description FROM tests")
    if cursor:
        tests = cursor.fetchall()
        return jsonify([{'test_id': row[0], 'test_name': row[1], 'description': row[2]} for row in tests])
    return jsonify({'message': 'Database error'}), 500

@app.route('/tests', methods=['POST'])
def create_test():
    data = request.get_json()
    # Валидация данных
    if not data.get('test_name') or not data.get('description') or not data.get('creator_id'):
        return jsonify({'message': 'Missing test_name, description, or creator_id'}), 400

    cursor = execute_query("INSERT INTO tests (test_name, description, creator_id) VALUES (%s, %s, %s)", (data['test_name'], data['description'], data['creator_id']))
    if cursor:
        return jsonify({'message': 'Test created'}), 201
    return jsonify({'message': 'Database error'}), 500

# ... (добавить GET, PUT, DELETE для /tests/<int:test_id> аналогично users)

if __name__ == '__main__':
    app.run(debug=True)
