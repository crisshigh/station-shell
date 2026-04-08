use sqlx::{postgres::PgPoolOptions, PgPool};
use std::time::Duration;

pub async fn creer_connexion() -> PgPool {
    let database_url = std::env::var("DATABASE_URL")
        .expect("DATABASE_URL doit être défini dans .env");

    PgPoolOptions::new()
        .max_connections(10)
        .acquire_timeout(Duration::from_secs(5))
        .connect(&database_url)
        .await
        .expect("Impossible de se connecter à PostgreSQL")
}