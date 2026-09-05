# دليل تجهيز GitHub وFirebase والاستضافة (Mondabet-Code)

هذا الدليل بيغطي الخطوات المتبقية اللي محتاجة حسابك الشخصي (GitHub وFirebase وOracle Cloud) - كل حاجة تانية جاهزة بالفعل في الكود (workflow البناء، docker-compose، Caddy، الـ Dockerfiles).

## أ. رفع المشروع على GitHub (مطلوب لبناء الـ APK تلقائيًا)

١. سجّل دخول على github.com واعمل ريبو جديد فاضي (اسمه مثلاً `mondabet-code`، عام أو خاص زي ما تحب).

٢. من جهازك (Terminal عادي، مش من هنا):
```bash
cd path\to\Mondabet-Code
git remote remove origin
git remote add origin https://github.com/<username>/mondabet-code.git
git push -u origin main
```

٣. بعد الـ push، افتح تبويب **Actions** في الريبو - هتلاقي تشغيل تلقائي لـ "Build mobile APK" بدأ. بياخد حوالي ٥-١٠ دقايق. لما يخلص، ملف الـ APK هيبقى جاهز للتنزيل تحت **Artifacts** في نفس الـ run.

## ب. ربط Firebase (اختياري - بس لازم عشان الإشعارات الفورية تشتغل)

بدون الخطوة دي، التطبيق هيشتغل عادي، بس من غير إشعارات push.

١. روح console.firebase.google.com وأنشئ مشروع جديد بحسابك.
٢. ضيف تطبيق Android بـ package name: `com.mondabet.mondabet` (ده الاسم اللي workflow البناء بيولده تلقائيًا؛ لو مختلف تقدر تتأكد من `android/app/build.gradle` بعد أول تشغيل).
٣. نزّل ملف `google-services.json`.
٤. حوّله لـ base64 (في PowerShell على ويندوز):
```powershell
[Convert]::ToBase64String([IO.File]::ReadAllBytes("google-services.json")) | Set-Clipboard
```
(دلوقتي النص متسوخ في الـ clipboard بتاعك)

٥. في إعدادات الريبو على GitHub: **Settings → Secrets and variables → Actions → New repository secret** - الاسم `GOOGLE_SERVICES_JSON`، والقيمة الصقها من الـ clipboard.
٦. أعد تشغيل الـ workflow يدويًا (Actions → Build mobile APK → Run workflow) عشان الإشعارات تشتغل في الـ APK الجديد.

## ج. استضافة المشروع كامل (Oracle Cloud Always Free + دومين)

١. اعمل حساب على cloud.oracle.com واختار Always Free.
٢. أنشئ VM: **Compute → Instances → Create Instance**، اختار الشكل Ampere (ARM) `VM.Standard.A1.Flex` بأعلى مواصفات متاحة لحسابك (Always Free بتديك حتى ٢-٤ أنوية و١٢-٢٤ جيجا حسب المنطقة)، والصورة Ubuntu (أحدث نسخة LTS).
٣. من إعدادات الشبكة (Security List أو Network Security Group)، افتح البورتات ٨٠ و٤٤٣ للعالم الخارجي.
٤. اتصل بالسيرفر عبر SSH ونصّب Docker:
```bash
curl -fsSL https://get.docker.com | sudo sh
sudo usermod -aG docker $USER
```
(يمكن تحتاج تعمل logout/login تاني عشان الصلاحية تتفعل)

٥. اجلب الكود (بعد ما تخلص خطوة أ فوق):
```bash
git clone https://github.com/<username>/mondabet-code.git
cd mondabet-code
cp .env.example .env
nano .env   # غيّر الباسوردات الافتراضية + حط DOMAIN و ACME_EMAIL
```

٦. الدومين - عندك خياران:
   - **دومين حقيقي**: وجّه سجلات DNS (A records) لـ `api.<domain>` و`app.<domain>` و`admin.<domain>` كلهم لنفس IP السيرفر.
   - **للتجربة السريعة من غير شراء دومين**: استخدم خدمة `sslip.io` المجانية - لو IP السيرفر هو مثلاً `1.2.3.4`، حط في `.env`:
     ```
     DOMAIN=1.2.3.4.sslip.io
     ```
     وهتقدر تفتح `https://app.1.2.3.4.sslip.io` مباشرة من غير أي إعداد DNS يدوي.

٧. شغّل الستاك كامل:
```bash
docker compose -f docker-compose.prod.yml up -d --build
```
أول تشغيل هياخد وقت (بناء ١٠ خدمات .NET + بورتالين من الصفر). تابع التقدم بـ:
```bash
docker compose -f docker-compose.prod.yml logs -f
```

٨. لما كل حاجة تبقى `healthy`، افتح `https://app.<domain>` في المتصفح.

## تنبيه أمني

مفاتيح JWT ومفاتيح Keycloak المدمجة في المشروع حاليًا هي مفاتيح تطوير معروفة (نفسها موجودة في الكود مش سرية) - مناسبة تمامًا للتجربة العملية، لكن قبل أي استخدام حقيقي ببيانات موظفين فعليين، محتاجين نولّد مفاتيح إنتاج جديدة وأسرار Keycloak حقيقية. قولّي لو حابب نعملها لما توصل للمرحلة دي.
