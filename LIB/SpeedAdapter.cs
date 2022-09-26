namespace BWM.LIB
{
    public class SpeedAdapter
    {
        public const int WT_1Second = 1;
        public const int WT_2Second = 2;
        public const int WT_3Second = 3;
        public const int WT_4Second = 4;
        public const int WT_5Second = 5;
        public const int WT_10Second = 10;
        public const int WT_20Second = 20;
        public const int WT_50Second = 50;
        public const int WT_1Minute = 1 * 60;
        public const int WT_2Minute = 2 * 60;
        public const int WT_5Minute = 5 * 60;
        public const int WT_10Minute = 10 * 60;
        public const int WT_15Minute = 15 * 60;
        public const int WT_20Minute = 20 * 60;
        public const int WT_30Minute = 30 * 60;
        public const int WT_50Minute = 50 * 60;
        public const int WT_1Hour = 1 * 60 * 60;
        public const int WT_2Hour = 2 * 60 * 60;
        public const int WT_3Hour = 3 * 60 * 60;
        public const int WT_4Hour = 4 * 60 * 60;
        public const int WT_5Hour = 5 * 60 * 60;
        public const int WT_6Hour = 6 * 60 * 60;
        public const int WT_7Hour = 7 * 60 * 60;
        public const int WT_8Hour = 8 * 60 * 60;
        public const int WT_9Hour = 9 * 60 * 60;
        public const int WT_10Hour = 10 * 60 * 60;
        public const int WT_11Hour = 11 * 60 * 60;
        public const int WT_12Hour = 12 * 60 * 60;

        public int currentSpeed = 0;

        private int[,] milestones; // Các speed có chỉ số từ 0: tốc độ chậm nhất - chỉ số 0; tốc độ nhanh nhất - chỉ số maxSpeed
        private int maxSize = 20;
        private int maxSpeed = 0;
        private int defaultSpeed = 0;

        public SpeedAdapter(int[,] ms)
        {
            milestones = ms;
            maxSpeed = milestones.GetLength(0) - 1;
            defaultSpeed = (maxSpeed + 1) / 2;
            currentSpeed = defaultSpeed;
        }

        public SpeedAdapter(int max)
        {
            maxSize = max;
            milestones = new int[maxSize, 2];
            maxSpeed = -1;
            currentSpeed = (maxSpeed + 1) / 2;
        }

        public int GetMaxSpeed() { return maxSpeed; }

        public int GetNumOfSpeeds() { return maxSpeed + 1; }

        public bool AddMilestone(int numOfWorks, int waitingTime)
        {
            if (maxSpeed >= maxSize - 1)
            {
                return false;
            }
            maxSpeed++;
            milestones[maxSpeed, 0] = numOfWorks;
            milestones[maxSpeed, 1] = waitingTime;
            defaultSpeed = (maxSpeed + 1) / 2;
            currentSpeed = defaultSpeed;

            return true;
        }

        public bool SetSpeed(int i)
        {
            if (i < 0 || i > maxSpeed) return false;

            currentSpeed = i;
            return true;
        }

        public bool SetDefaultSpeed(int i)
        {
            if (i < 0 || i > maxSpeed) return false;

            defaultSpeed = i;
            return true;
        }

        public bool IncreaseSpeed()
        {
            if (currentSpeed >= maxSpeed) return false;

            if (currentSpeed <= 0) currentSpeed = defaultSpeed;
            else currentSpeed++;

            return true;
        }

        public bool DecreaseSpeed()
        {
            if (currentSpeed <= 0) return false;

            currentSpeed--;

            return true;
        }

        public int GetNumOfWorks()
        {
            return milestones[currentSpeed, 0];
        }

        public int GetWaitingTime()
        {
            return milestones[currentSpeed, 1];
        }

        public int GetCurrentSpeed()
        {
            return currentSpeed;
        }

        public string GetCurrentSpeedString(string name = "")
        {
            if (name == "") name = "work";

            return "#" + (currentSpeed + 1) + "/" + (maxSpeed + 1) + " [" + GetNumOfWorks() + " " + name + "(s) / " + GetWaitingTime() + " second(s)]";
        }
    }
}