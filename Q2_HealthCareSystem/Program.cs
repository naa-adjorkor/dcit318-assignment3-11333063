namespace Q2_HealthCareSystem;

// a) Generic Repository
    public class Repository<T>
    {
        private readonly List<T> _items = new();

        public void Add(T item)
        {
            _items.Add(item);
        }

        public List<T> GetAll()
        {
            return new List<T>(_items);
        }

        public T? GetById(Func<T, bool> predicate)
        {
            return _items.FirstOrDefault(predicate);
        }

    public bool Remove(Func<T, bool> predicate)
    {
        var item = _items.FirstOrDefault(predicate);
        if (item != null)
        {
            _items.Remove(item);
            return true;
        }
        return false;
    }
}

// b) Patient
    public class Patient
    {
        public int Id { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }

        public override string ToString()
        {
            return $"[Patient] ID: {Id}, Name: {Name}, Age: {Age}, Gender: {Gender}";
        }
    }

// c) Prescription
    public class Prescription
    {
        public int Id { get; }
        public int PatientId { get; }
        public string MedicationName { get; }
        public DateTime DateIssued { get; }

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }

        public override string ToString()
        {
            return $"[Prescription] ID: {Id}, PatientID: {PatientId}, Medication: {MedicationName}, Date: {DateIssued:d}";
        }
    }

// g) HealthSystemApp
    public class HealthSystemApp
    {
        private readonly Repository<Patient> _patientRepo = new();
        private readonly Repository<Prescription> _prescriptionRepo = new();
        private readonly Dictionary<int, List<Prescription>> _prescriptionMap = new();

        // Seed data
        public void SeedData()
        {
            _patientRepo.Add(new Patient(1, "Bernice Adjei", 30, "Female"));
            _patientRepo.Add(new Patient(2, "Wisdom Kporha", 45, "Male"));
            _patientRepo.Add(new Patient(3, "Benedicta Akrong", 28, "Female"));

            _prescriptionRepo.Add(new Prescription(1, 1, "Amoxicillin", DateTime.Today.AddDays(-10)));
            _prescriptionRepo.Add(new Prescription(2, 1, "Paracetamol", DateTime.Today.AddDays(-5)));
            _prescriptionRepo.Add(new Prescription(3, 2, "Ibuprofen", DateTime.Today.AddDays(-7)));
            _prescriptionRepo.Add(new Prescription(4, 3, "Cetirizine", DateTime.Today.AddDays(-3)));
            _prescriptionRepo.Add(new Prescription(5, 1, "Vitamin C", DateTime.Today));
        }

        // Build prescription map
        public void BuildPrescriptionMap()
        {
            _prescriptionMap.Clear();
            foreach (var prescription in _prescriptionRepo.GetAll())
            {
                if (!_prescriptionMap.ContainsKey(prescription.PatientId))
                    _prescriptionMap[prescription.PatientId] = new List<Prescription>();

                _prescriptionMap[prescription.PatientId].Add(prescription);
            }
        }

        // Print all patients
        public void PrintAllPatients()
        {
            Console.WriteLine("=== All Patients ===");
            foreach (var patient in _patientRepo.GetAll())
            {
                Console.WriteLine(patient);
            }
        }

        // Print prescriptions for a specific patient
        public void PrintPrescriptionsForPatient(int patientId)
        {
            Console.WriteLine($"\n=== Prescriptions for Patient ID: {patientId} ===");
            if (_prescriptionMap.TryGetValue(patientId, out var prescriptions))
            {
                foreach (var p in prescriptions)
                {
                    Console.WriteLine(p);
                }
            }
            else
            {
                Console.WriteLine("No prescriptions found for this patient.");
            }
    }

        // Run flow
        public void Run()
        {
            SeedData();
            BuildPrescriptionMap();
            PrintAllPatients();

            Console.Write("\nEnter Patient ID to view prescriptions: ");
                if (int.TryParse(Console.ReadLine(), out int pid))
                {
                    PrintPrescriptionsForPatient(pid);
                }
                else
                {
                    Console.WriteLine("Invalid input.");
                }
        }
    }

    // Program entry
     public static class Program
     {
        public static void Main()
        {
            var app = new HealthSystemApp();
            app.Run();
        }
     }