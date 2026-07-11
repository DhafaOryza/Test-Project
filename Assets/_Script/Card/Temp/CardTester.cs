using UnityEngine;

// =======================================================
// 1. KELAS INDUK (Cetak Biru Utama / Abstract)
// =======================================================
public abstract class BaseCard 
{
    // Deklarasi HANYA 1 KALI di sini
    public string Title; 
    public int Cost;     

    // Guard / Syarat wajib untuk semua anak
    public abstract void PlayCardEffect(); 
}


// =======================================================
// 2. KELAS ANAK 1 (Kartu Serangan)
// =======================================================
public class SwordCard : BaseCard 
{
    // Keahlian spesifik, cuma kartu ini yang punya "Damage"
    public int Damage; 

    // Memenuhi syarat dari induk menggunakan kata kunci "override"
    public override void PlayCardEffect() 
    {
        // Lihat! Dia bisa memanggil "Title" dan "Cost" tanpa perlu mendeklarasikannya di kelas ini!
        Debug.Log($"Serang!! Kartu {Title} dimainkan (Harga: {Cost} energi). Memberikan {Damage} damage ke musuh!");
    }
}


// =======================================================
// 3. KELAS ANAK 2 (Kartu Bertahan)
// =======================================================
public class PotionCard : BaseCard 
{
    // Keahlian spesifik, cuma kartu ini yang punya "HealAmount"
    public int HealAmount; 

    // Memenuhi syarat dari induk
    public override void PlayCardEffect() 
    {
        Debug.Log($"Segar!! Kartu {Title} dimainkan (Harga: {Cost} energi). Menyembuhkan {HealAmount} HP!");
    }
}


// =======================================================
// 4. SCRIPT PENGUJI (Tempat kita bermain-main)
// =======================================================
public class CardTester : MonoBehaviour
{
    void Start()
    {
        // A. Membuat Kartu Pedang
        SwordCard kartuPedang = new SwordCard();
        kartuPedang.Title = "Pedang Naga"; // Mengisi variabel milik Induk
        kartuPedang.Cost = 2;              // Mengisi variabel milik Induk
        kartuPedang.Damage = 50;           // Mengisi variabel Spesifik miliknya sendiri

        // B. Membuat Kartu Ramuan
        PotionCard kartuRamuan = new PotionCard();
        kartuRamuan.Title = "Ramuan Suci";
        kartuRamuan.Cost = 1;
        kartuRamuan.HealAmount = 30;

        // C. MEMAINKAN KARTU (Ini akan memanggil efek mereka masing-masing)
        kartuPedang.PlayCardEffect();
        kartuRamuan.PlayCardEffect();
    }
}