mod db;

use axum::{routing::get, Router, extract::State};
use std::net::SocketAddr;
use sqlx::PgPool;

#[tokio::main]
async fn main() {
    // Initialiser les logs
    tracing_subscriber::fmt::init();

    // Charger les variables d'environnement
    dotenvy::dotenv().ok();

    let port = std::env::var("PORT")
        .unwrap_or("3000".to_string())
        .parse::<u16>()
        .expect("PORT invalide");

    // Connexion à la base de données
    let pool = db::creer_connexion().await;
    tracing::info!("✅ Connecté à PostgreSQL !");

    // Définir les routes
    let app = Router::new()
        .route("/", get(root))
        .route("/health", get(health_check))
        .with_state(pool);

    // Démarrer le serveur
    let addr = SocketAddr::from(([127, 0, 0, 1], port));
    tracing::info!("🚀 Serveur démarré sur http://{}", addr);

    let listener = tokio::net::TcpListener::bind(addr).await.unwrap();
    axum::serve(listener, app).await.unwrap();
}

async fn root() -> &'static str {
    "🚀 Gestion Station Shell — API opérationnelle"
}

async fn health_check(State(pool): State<PgPool>) -> String {
    match sqlx::query("SELECT 1")
        .execute(&pool)
        .await
    {
        Ok(_) => "✅ API OK — Base de données connectée !".to_string(),
        Err(e) => format!("❌ Erreur base de données : {}", e),
    }
}